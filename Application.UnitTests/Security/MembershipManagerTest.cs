using System;
using System.Linq;
using Application.Diagnostics;
using Application.Eventing;
using Application.Security;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Application.UnitTests.Security
{
  public class MembershipManagerTest
  {
    [Theory]
    [InlineAutoNSubstituteData(null)]
    [InlineAutoNSubstituteData("")]
    public void ShouldFailForNullOrEmptyId(string userName, MembershipManager sut, string password)
    {
      // Act & Assert
      sut.Invoking(s => s.TryLogin(userName, password)).ShouldThrow<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldFailForNullPassword(MembershipManager sut, string login)
    {
      // Act & Assert
      sut.Invoking(s => s.TryLogin(login, null)).ShouldThrow<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldNotFailForEmptyPassword(MembershipManager sut, string login)
    {
      // Arrange
      var emptyPassword = "";
      // Act & Assert
      sut.Invoking(s => s.TryLogin(login, emptyPassword)).ShouldNotThrow();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditAttempt([Frozen] ILog log, [Greedy] MembershipManager sut, string login, string password)
    {
      // Arrange
      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Trying to log in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLookupUserByLogin([Frozen] IUserRepository repo, MembershipManager sut, string login, string password)
    {
      // Arrange
      // Act
      sut.TryLogin(login, password);

      // Assert
      repo.Received().GetExistingUser(login);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldReturnFalseIfUserDoesNotExist([Frozen] IUserRepository repo, MembershipManager sut, string login, string password)
    {
      // Arrange
      repo.GetExistingUser(login).ReturnsNull();

      // Act
      var result = sut.TryLogin(login, password);

      // Assert
      result.Should().BeFalse();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldReturnFalseIfPasswordIsInvalid([Frozen] IPasswordChecker passwordChecker, MembershipManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(false);

      // Act
      var result = sut.TryLogin(login, password);

      // Assert
      result.Should().BeFalse();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLoginIfPasswordIsValid([Frozen] IUserRepository repo, [Frozen] IPasswordChecker passwordChecker, MembershipManager sut, User user, string login, string password)
    {
      // Arrange
      repo.GetExistingUser(login).Returns(user);
      passwordChecker.IsPasswordValid(password, user).Returns(true);

      // Act
      var result = sut.TryLogin(login, password);

      // Assert
      result.Should().BeTrue();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditIfLoginFailedDueToInvalidPass([Frozen] ILog log, [Frozen] IPasswordChecker passwordChecker, [Greedy] MembershipManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(false);

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Failed to log in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditIfLoginFailedDueToNonExistingUser([Frozen] ILog log, [Frozen] IUserRepository repo, [Greedy] MembershipManager sut, string login, string password)
    {
      // Arrange
      repo.GetExistingUser(Arg.Any<string>()).ReturnsNull();

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Failed to log in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditIfLoggedIn([Frozen] ILog log, [Frozen] IPasswordChecker passwordChecker, [Greedy] MembershipManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(true);

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Logged in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldFailForNullUsersDuringCreation(MembershipManager sut)
    {
      // Arrange
      // Act & Assert
      sut.Invoking(s => s.CreateMultipleUsers(null)).ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineAutoNSubstituteData(0)]
    [InlineAutoNSubstituteData(3)]
    [InlineAutoNSubstituteData(7)]
    [InlineAutoNSubstituteData(10)]
    public void ShouldNotFailCreationForLessThan10Users(int count, MembershipManager sut, IFixture fixture)
    {
      // Arrange
      var users = fixture.CreateMany<User>(count).ToArray();

      // Act & Assert
      sut.Invoking(s => s.CreateMultipleUsers(users)).ShouldNotThrow();
    }

    [Theory]
    [InlineAutoNSubstituteData(11)]
    [InlineAutoNSubstituteData(20)]
    [InlineAutoNSubstituteData(50)]
    public void ShouldFailForMoreThan10Users(int count, MembershipManager sut, IFixture fixture)
    {
      // Arrange
      var users = fixture.CreateMany<User>(count).ToArray();

      // Act & Assert
      sut.Invoking(s => s.CreateMultipleUsers(users))
        .ShouldThrow<ArgumentException>()
        .WithMessage("*10 users*");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldNotVerifyUserDataIsFilled([Frozen] IUserRepository repo, MembershipManager sut, [NoAutoProperties] User user)
    {
      // Arrange
      var users = new[] {user};

      // Act
      sut.CreateMultipleUsers(users);

      // Assert
      repo.Received().CreateUser(user);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldCreateEachUser([Frozen] IUserRepository repo, MembershipManager sut, User[] users)
    {
      // Arrange
      // Act
      sut.CreateMultipleUsers(users);

      // Assert
      Assert.All(users, u => repo.Received().CreateUser(u));
    }

    [Theory, AutoNSubstituteData]
    public void ShouldEmitEventIfLoggedIn([Frozen] IEventEmitter evEmitter, [Frozen] IPasswordChecker passwordChecker, MembershipManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(true);
      // Act
      sut.TryLogin(login, password);

      // Assert
      evEmitter.Received().EmitUserLoggedIn(login);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldNotEmitEventsIfUserFailedToLogIn([Frozen] IEventEmitter evEmitter, [Frozen] IPasswordChecker passwordChecker, MembershipManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(false);

      // Act
      sut.TryLogin(login, password);

      // Assert
      evEmitter.DidNotReceive().EmitUserLoggedIn(Arg.Any<string>());
    }
  }
}