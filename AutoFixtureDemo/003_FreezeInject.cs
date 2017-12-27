using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace AutoFixtureDemo
{
  public class FreezeInject
  {
    #region Inject

    [Fact]
    public void Inject_Demo()
    {
      var fixture = new Fixture();

      fixture.Inject<int>(42);

      var value1 = fixture.Create<int>();
      var value2 = fixture.Create<int>();

      Assert.Equal(42, value1);
      Assert.Equal(42, value2);
    }

    #endregion

    #region Freeze

    [Fact]
    public void Freeze_Demo()
    {
      var fixture = new Fixture();

      /*var frozenValue = fixture.Create<string>();
      fixture.Inject<string>(frozenValue);*/

      var frozenValue = fixture.Freeze<string>();

      var arrayOfStrings = fixture.Create<string[]>();
      Assert.True(arrayOfStrings.All(x => x == frozenValue));
    }

    #endregion

    #region Complex freeze demo

    public class Logger
    {
      private List<string> messages = new List<string>();

      public IReadOnlyList<string> Messages => messages;

      public void Log(string msg) => messages.Add(msg);
    }

    public class SelfLoggingType
    {
      private readonly Logger _logger;

      public string StringValue { get; }
      public int IntValue { get; }
      public bool BoolValue { get; }

      public SelfLoggingType(string stringValue, int intValue, bool boolValue, Logger logger)
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

    [Fact]
    public void ShouldLogValidNumber()
    {
      // Arrange
      var fixture = new Fixture();

      var logger = fixture.Freeze<Logger>();
      var sut = fixture.Create<SelfLoggingType>();

      // Act
      sut.LogIntValue();

      // Assert
      var allMessages = logger.Messages;
      Assert.Contains(sut.IntValue.ToString(), allMessages);
    }

    #endregion
  }
}
