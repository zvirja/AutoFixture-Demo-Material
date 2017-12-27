using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Security
{
  public interface IPasswordChecker
  {
    bool IsPasswordValid(string password, User user);
  }
}
