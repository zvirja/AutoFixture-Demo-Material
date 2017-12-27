using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Security
{
  public class User
  {
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
  }
}
