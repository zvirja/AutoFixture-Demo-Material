namespace Application.Security
{
  public class User
  {
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
  }
}
