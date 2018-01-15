using System;
using Application.Diagnostics;
using Application.Eventing;

namespace Application.Security
{
  public class MembershipManager
  {
    private readonly IUserRepository _userRepo;
    private readonly IPasswordChecker _passwordChecker;
    private readonly IEventEmitter _eventEmitter;
    private readonly ILog _log;

    public MembershipManager(IUserRepository userRepo, IPasswordChecker passwordChecker, IEventEmitter eventEmitter)
    : this(userRepo, passwordChecker, eventEmitter, new NullLog())
    {
    }

    public MembershipManager(IUserRepository userRepo, IPasswordChecker passwordChecker, IEventEmitter eventEmitter, ILog log)
    {
      _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
      _passwordChecker = passwordChecker ?? throw new ArgumentNullException(nameof(passwordChecker));
      _eventEmitter = eventEmitter ?? throw new ArgumentNullException(nameof(eventEmitter));
      _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public bool TryLogin(string login, string password)
    {
      if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));
      if (password == null) throw new ArgumentNullException(nameof(password));

      _log.Audit(login, "Trying to log in.");

      var user = _userRepo.GetExistingUser(login);
      if (user != null && _passwordChecker.IsPasswordValid(password, user))
      {
        _log.Audit(login, "Logged in.");
        _eventEmitter.EmitUserLoggedIn(login);

        return true;
      }

      _log.Audit(login, "Failed to log in.");
      return false;
    }

    public void CreateMultipleUsers(User[] users)
    {
      if (users == null) throw new ArgumentNullException(nameof(users));

      if(users.Length > 10) throw new ArgumentException("Too many users. You cannot create more than 10 users at once.");

      foreach (var user in users)
      {
        _userRepo.CreateUser(user);
      }
    }
  }
}
