using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;

namespace AutoFixtureDemo
{
  public class XUnitIntegration
  {
    #region AutoDataDemo

    [Theory, AutoData]
    public void ShouldGenerateValuesInParameters(int number, Guid id, string name, TimeSpan duration)
    {
      Assert.NotEqual(0, number);
      Assert.NotEqual(Guid.Empty, id);
      Assert.NotNull(name);
      Assert.NotEqual(TimeSpan.Zero, duration);
    }

    #endregion

    #region AutoData for complex types

    public class MessageDecorator
    {
      public string MessagePrefix { get; set; }

      public string GetPrefixedMessage(string msg) => $"{MessagePrefix}_{msg}";
    }

    [Theory, AutoData]
    public void ShouldDecorateMessageWithPrefix(MessageDecorator sut, string message)
    {
      var result = sut.GetPrefixedMessage(message);

      Assert.StartsWith(sut.MessagePrefix, result);
      Assert.EndsWith(message, result);
    }

    #endregion

    #region InlineAutoData

    public class NumericValueTranslator
    {
      public string GetTranslatedMessage(int number, string fallbackValue)
      {
        switch (number)
        {
          case 1: return "one";
          case 2: return "two";
          case 3: return "three";

          default:
            return fallbackValue;
        }
      }
    }

    [Theory]
    [InlineAutoData(1, "one")]
    [InlineAutoData(2, "two")]
    [InlineAutoData(3, "three")]
    public void ShouldCorrectlyTranslateNumbers(int numericValue, string translatedValue, NumericValueTranslator sut, string fallback)
    {
      var result = sut.GetTranslatedMessage(numericValue, fallback);

      Assert.Equal(translatedValue, result);
    }

    #endregion

    #region Frozen

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

    [Theory, AutoData]
    public void ShouldLogValidNumber(/*[Frozen]*/ Logger logger, SelfLoggingType sut)
    {
      // Act
      sut.LogIntValue();

      // Assert
      var allMessages = logger.Messages;
      //Assert.Contains(sut.IntValue.ToString(), allMessages);
    }

    #endregion

    #region Greedy

    public class OwnedValueWrapper<T>
    {
      public T Value { get; }
      public object Owner { get; }

      public OwnedValueWrapper(T value):
        this(value, owner: null)
      {
      }

      public OwnedValueWrapper(T value, object owner)
      {
        this.Value = value;
        this.Owner = owner;
      }
    }

    [Theory, AutoData]
    public void GreedyDemo(/*[Greedy]*/ OwnedValueWrapper<int> value)
    {
      //Assert.NotNull(value.Owner);
    }

    #endregion
  }
}
