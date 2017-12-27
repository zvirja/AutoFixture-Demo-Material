namespace Application.Diagnostics
{
  public class NullLog : ILog
  {
    public void Audit(string user, string action)
    {
    }
  }
}