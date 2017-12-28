using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;

namespace AutoFixtureDemo
{
  public class NSubstituteIntegration
  {
    public class ValueSource
    {
      public virtual string Value { get; }

      public ValueSource(string value)
      {
        this.Value = value;
      }
    }

    public interface IValueRepository
    {
      ValueSource GetValue(string id);
    }

    #region NSubstitute integration

    /* To demo:
     * 1. Auto vs AutoConfigured 
     * 2. Different input - different output
     */
    [Fact]
    public void IntegrationDemo()
    {
      var fixture = new Fixture();
      fixture.Customize(new AutoNSubstituteCustomization());
      //fixture.Customize(new AutoConfiguredNSubstituteCustomization());

      var repo = fixture.Create<IValueRepository>();

      var valueSource = repo.GetValue("foo");
      //Assert.NotNull(valueSource);
    }

    #endregion

    #region NSubstitute + XUnit

    public class AutoNSubstituteData : AutoDataAttribute
    {
      public AutoNSubstituteData():
        base(() => new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
      {
      }
    }

    /* To demo:
     * 1. Frozen ValueSource
     */
    [Theory, AutoNSubstituteData]
    public void DemoXUnitIntegration(IValueRepository repo)
    {
      var valueSource = repo.GetValue("foo");

      Assert.NotNull(valueSource);
    }


    #endregion

    #region Substitute attribute

    /* To demo:
     * 1. Combined with frozen 
     */
    [Theory, AutoNSubstituteData]
    public void ShouldCreateSubstitute([Substitute] ValueSource value)
    {
      value.Value.Returns("foo");

      Assert.Equal("foo", value.Value);
    }

    #endregion

    #region Freeze recup with NSubstitute + XUnit

    public interface ILogger
    {
      void Log(string msg);
    }

    public class SelfLoggingType
    {
      private readonly ILogger _logger;

      public string StringValue { get; }
      public int IntValue { get; }
      public bool BoolValue { get; }

      public SelfLoggingType(string stringValue, int intValue, bool boolValue, ILogger logger)
      {
        StringValue = stringValue;
        IntValue = intValue;
        BoolValue = boolValue;

        _logger = logger;
      }

      public void LogStringValue() => _logger.Log(StringValue);
      public void LogIntValue() => _logger.Log(IntValue.ToString());
      public void LogByteValue() => _logger.Log(BoolValue.ToString());
    }

    [Theory, AutoNSubstituteData]
    public void ShouldLogValidNumber([Frozen] ILogger logger, SelfLoggingType sut)
    {
      // Act
      sut.LogIntValue();

      // Assert
      logger.Received().Log(sut.IntValue.ToString());
    }

    #endregion
  }
}
