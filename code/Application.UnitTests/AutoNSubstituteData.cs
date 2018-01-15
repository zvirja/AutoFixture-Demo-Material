using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Application.UnitTests
{
  public class AutoNSubstituteData : AutoDataAttribute
  {
    public AutoNSubstituteData(): base(() => 
      
      new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
    {
    }
  }
}
