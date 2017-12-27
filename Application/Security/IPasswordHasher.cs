using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Security
{
  public interface IPasswordHasher
  {
    (string value, string salt) HashNewPassword(string password);
    string HashPasswordWithSalt(string password, string salt);
  }
}
