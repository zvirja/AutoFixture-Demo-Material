using System;
using Application.Diagnostics;
using Application.Security;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Application.UnitTests.Security
{
  public class LoginManagerTest
  {
    [Theory, AutoNSubstituteData]
    public void ShouldFailForNullId(LoginManager sut, string password)
    {
      // Act & Assert
      sut.Invoking(s => s.TryLogin(null, password)).ShouldThrow<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldFailForNullPassword(LoginManager sut, string login)
    {
      // Act & Assert
      sut.Invoking(s => s.TryLogin(login, null)).ShouldThrow<ArgumentNullException>();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldNotFailForEmptyPassword(LoginManager sut, string login)
    {
      // Arrange
      var emptyPassword = "";
      // Act & Assert
      sut.Invoking(s => s.TryLogin(login, emptyPassword)).ShouldNotThrow();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditAttempt([Frozen] ILog log, [Greedy] LoginManager sut, string login, string password)
    {
      // Arrange
      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Try to login.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLookupUserByLogin([Frozen] IUserRepository repo, LoginManager sut, string login, string password)
    {
      // Arrange
      // Act
      sut.TryLogin(login, password);

      // Assert
      repo.Received().GetExistingUser(login);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldReturnFalseIfUserDoesNotExist([Frozen] IUserRepository repo, LoginManager sut, string login, string password)
    {
      // Arrange
      repo.GetExistingUser(login).ReturnsNull();

      // Act
      var result = sut.TryLogin(login, password);

      // Assert
      result.Should().BeFalse();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldReturnFalseIfPasswordIsInvalid([Frozen] IPasswordChecker passwordChecker, LoginManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(false);

      // Act
      var result = sut.TryLogin(login, password);

      // Assert
      result.Should().BeFalse();
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLoginIfPasswordIsValid([Frozen] IUserRepository repo, [Frozen] IPasswordChecker passwordChecker, LoginManager sut, User user, string login, string password)
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
    public void ShouldLogAuditIfLoginFailedDueToInvalidPass([Frozen] ILog log, [Frozen] IPasswordChecker passwordChecker, [Greedy] LoginManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(false);

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Failed to log in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditIfLoginFailedDueToNonExistingUser([Frozen] ILog log, [Frozen] IUserRepository repo, [Greedy] LoginManager sut, string login, string password)
    {
      // Arrange
      repo.GetExistingUser(Arg.Any<string>()).ReturnsNull();

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Failed to log in.");
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogAuditIfLoggedIn([Frozen] ILog log, [Frozen] IPasswordChecker passwordChecker, [Greedy] LoginManager sut, string login, string password)
    {
      // Arrange
      passwordChecker.IsPasswordValid(Arg.Any<string>(), Arg.Any<User>()).Returns(true);

      // Act
      sut.TryLogin(login, password);

      // Assert
      log.Received().Audit(login, "Logged in.");
    }
  }
}
