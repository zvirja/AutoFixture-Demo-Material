namespace Application.Security
{
  public interface IUserRepository
  {
    User GetExistingUser(string userName);

    void CreateUser(User user);
  }
}
