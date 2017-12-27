using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Diagnostics
{
  public interface ILog
  {
    void Audit(string user, string action);
  }
}
