using System;
using System.Collections.Generic;
using AutoFixture;
using Xunit;

namespace AutoFixtureDemo
{
  public class Primitives
  {
    [Fact]
    public void ShouldCreatePrimitives()
    {
      var fixture = new Fixture();

      var number = fixture.Create<int>();
      var guid = fixture.Create<Guid>();
      var datetime = fixture.Create<DateTime>();
      var uri = fixture.Create<Uri>();
      var enumValue = fixture.Create<PlatformID>();

      var array = fixture.Create<int[]>();
      var enumerable = fixture.Create<IEnumerable<byte>>();
      var list = fixture.Create<List<int>>();
      var dictionary = fixture.Create<Dictionary<byte, string>>();
    }
  }
}