using System;
using AutoFixture;
using Xunit;

namespace AutoFixtureDemo
{
  public class CustomTypes
  {
    #region NodeWithoutConstructor

    public class NodeDto
    {
      public string Value { get; set; }
      public int Index;
      public object ReadOnlyTag { get; } = null;
    }

    [Fact]
    public void ShouldCreateObjectWithoutConstructor()
    {
      var fixture = new Fixture();

      var dto = fixture.Create<NodeDto>();

      Assert.NotNull(dto);
      Assert.NotNull(dto.Value);
      Assert.NotEqual(0, dto.Index);
      Assert.Null(dto.ReadOnlyTag);
    }

    #endregion

    #region NodeWithContructor

    public class NodeWithCtor
    {
      public string Value { get; }
      public int Index { get; }
      public object SettableTag { get; set; }

      public NodeWithCtor(string value, int index)
      {
        Value = value;
        Index = index;
      }

/*      public NodeWithCtor(string value, int index, int fortyTwo)
      {
        if (fortyTwo != 42) throw new ArgumentOutOfRangeException(nameof(fortyTwo), fortyTwo, "Expected to be 42.");

        Value = value;
        Index = index;
      }*/
    }

    [Fact]
    public void ShouldCreateObjectWithConstructor()
    {
      var fixture = new Fixture();

      var dto = fixture.Create<NodeWithCtor>();

      Assert.NotNull(dto);
      Assert.NotNull(dto.Value);
      Assert.NotEqual(0, dto.Index);
      Assert.NotNull(dto.SettableTag);
    }

    #endregion

    #region WrapperAndNestedNode

    [Fact]
    public void ShouldCreateNestedMembers()
    {
      var fixture = new Fixture();

      var result = fixture.Create<WrappedNode<NodeWithCtor>>();

      Assert.NotNull(result.WrappedValue);
      Assert.NotNull(result.Owner);
      Assert.NotEqual(TimeSpan.Zero, result.Duration);
    }

    public class WrappedNode<T>
    {
      public T WrappedValue { get; }
      public object Owner { get; }
      public TimeSpan Duration { get; }


      public WrappedNode(T value, object owner, TimeSpan duration)
      {
        WrappedValue = value;
        Owner = owner;
        Duration = duration;
      }
    }

    #endregion
  }
}
