using System;
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
  }
}
