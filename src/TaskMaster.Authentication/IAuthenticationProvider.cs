namespace TaskMaster.Authentication;

public interface IAuthenticationProvider
{
  Task<Credential> LoginAsync();
}
