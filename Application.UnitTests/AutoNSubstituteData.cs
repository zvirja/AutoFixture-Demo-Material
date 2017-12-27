using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Application.UnitTests
{
  [AttributeUsage(AttributeTargets.Method)]
  public class AutoNSubstituteData : AutoDataAttribute
  {
    public AutoNSubstituteData()
      : base(() => new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
    {
    }
  }
}
