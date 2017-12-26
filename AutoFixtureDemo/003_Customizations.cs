using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;

namespace AutoFixtureDemo
{
  public class Customizations
  {
    #region Simple customization demo

    public class TestType
    {
      public readonly bool WasInitializedFromConstructor = false;
      private readonly bool _checkIntValue = true;

      private int _intValue;
      public int IntValue
      {
        get => _intValue;
        set
        {
          if (_checkIntValue && value != 42) throw new ArgumentOutOfRangeException(nameof(value), value, "first value should be 42");
          _intValue = value;
        }
      }

      public string StrValue { get; set; }
      public object ObjValue { get; set; }

      public TestType()
      {
      }

      public TestType(bool checkIntValue)
      {
        WasInitializedFromConstructor = true;
        _checkIntValue = checkIntValue;
      }
    }

    [Fact]
    public void Customize_Demo()
    {
      var fixture = new Fixture();
      
      //var result = fixture.Create<TestType>();

      //Assert.NotNull(result);
    }

    [Fact]
    public void Build_Demo()
    {
      var fixture = new Fixture();

      var result = fixture
        .Build<TestType>()
        .With(x => x.IntValue, 42)
        .Create();

      Assert.NotNull(result);

      // var result2 = fixture.Create<TestType>();
    }

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
    public void Freeze_Complex_Demo()
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
