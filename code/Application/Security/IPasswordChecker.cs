namespace Application.Security
{
  public interface IPasswordChecker
  {
    bool IsPasswordValid(string password, User user);
  }
}
