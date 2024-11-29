using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Authentication;
using TaskMaster.Authentication.Msal;

namespace TaskMaster.DependencyInjection;

public static class AuthenticationExtensions
{
  public static TaskMasterBuilder WithMsal(this TaskMasterBuilder taskMasterBuilder, string clientId, string authority)
  {
    taskMasterBuilder.ServiceCollection.AddSingleton<IAuthenticationProvider>(new MsalAuthenticationProvider(clientId, authority));
    return taskMasterBuilder;
  }
}
