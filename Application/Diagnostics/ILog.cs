namespace Application.Diagnostics
{
  public interface ILog
  {
    void Audit(string user, string action);
  }
}
