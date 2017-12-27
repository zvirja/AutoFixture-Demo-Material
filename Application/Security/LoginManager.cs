using System;
using Application.Diagnostics;

namespace Application.Security
{
  public class LoginManager
  {
    private readonly IUserRepository _userRepo;
    private readonly IPasswordChecker _passwordChecker;
    private readonly ILog _log;

    public LoginManager(IUserRepository userRepo, IPasswordChecker passwordChecker)
    : this(userRepo, passwordChecker, new NullLog())
    {
    }

    public LoginManager(IUserRepository userRepo, IPasswordChecker passwordChecker, ILog log)
    {
      _userRepo = userRepo;
      _passwordChecker = passwordChecker;
      _log = log;
    }

    public bool TryLogin(string login, string password)
    {
      if (login == null) throw new ArgumentNullException(nameof(login));
      if (password == null) throw new ArgumentNullException(nameof(password));

      _log.Audit(login, "Try to login.");

      var user = _userRepo.GetExistingUser(login);
      if (user != null && _passwordChecker.IsPasswordValid(password, user))
      {
        _log.Audit(login, "Logged in.");
        return true;
      }

      _log.Audit(login, "Failed to log in.");
      return false;
    }
  }
}
