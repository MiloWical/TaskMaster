using Microsoft.Identity.Client;

namespace TaskMaster.Authentication.Msal;

public class MsalAuthenticationProvider(string clientId, string authority) : IAuthenticationProvider
{
  //Constant value to target Azure DevOps. Do not change  
  private readonly string[] _scopes = ["499b84ac-1321-427f-aa17-267ca6975798/user_impersonation"];

  private readonly string _clientId = clientId;
  private readonly string _authority = authority;

    public async Task<Credential> LoginAsync()
  {
    // Initialize the MSAL library by building a public client application
    var application = PublicClientApplicationBuilder.Create(_clientId)
                               .WithAuthority(_authority)
                               .WithDefaultRedirectUri()
                               .Build();

    AuthenticationResult result;

    try
    {
      var accounts = await application.GetAccountsAsync();
      result = await application.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
              .ExecuteAsync();
    }
    catch (MsalUiRequiredException ex)
    {
      // If the token has expired, prompt the user with a login prompt
      result = await application.AcquireTokenInteractive(_scopes)
              .WithClaims(ex.Claims)
              .ExecuteAsync();
    }

    return new Credential
    {
      Value = result.CreateAuthorizationHeader()
    };
  }
}
