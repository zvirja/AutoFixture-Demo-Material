using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;

namespace Application.UnitTests
{
  public class InlineAutoNSubstituteData : InlineAutoDataAttribute
  {
    public InlineAutoNSubstituteData(params object[] values)
      : base(new AutoNSubstituteData(), values)
    {
    }
  }
}
