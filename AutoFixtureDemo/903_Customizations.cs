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
      public readonly bool WasInitializedFromVerboseConstructor = false;
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
        WasInitializedFromVerboseConstructor = true;
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

    #endregion

  }
}
