// // See https://aka.ms/new-console-template for more information

// using System.Diagnostics;
// using System.Text;
// using System.Text.Json.Nodes;

// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
// Console.OutputEncoding = Encoding.UTF8;

// using var process = new Process();

// process.StartInfo.FileName = "powershell";
// process.StartInfo.Arguments = "-C \"az boards work-item show --id 12345\"";
// process.StartInfo.UseShellExecute = false;  
// process.StartInfo.RedirectStandardOutput = true;
// process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
// process.StartInfo.EnvironmentVariables["DOTNET_CLI_FORCE_UTF8_ENCODING"] = "1";
// //process.StartInfo.RedirectStandardError = true;
// //process.StartInfo.WorkingDirectory = @"C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin";

// process.Start();

// var output = process.StandardOutput.ReadToEnd();
// //var _ = process.StandardError.ReadToEnd();
// process.WaitForExit();

// var json = JsonNode.Parse(output)!;

// Console.WriteLine(json["id"]);

using Microsoft.Identity.Client;
using System;
//using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

//
// The Client ID is used by the application to uniquely identify itself to Azure AD.
// The Tenant is the name or Id of the Azure AD tenant in which this application is registered.
// The AAD Instance is the instance of Azure, for example public Azure or Azure China.
// The Authority is the sign-in URL of the tenant.
//

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

var configuration = builder.Build();
string aadInstance = configuration["msal:AADInstance"];
string tenant = configuration["msal:Tenant"];
string clientId = configuration["msal:ClientId"];
string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

Console.WriteLine("Azure AD Instance: " + aadInstance);
Console.WriteLine("Azure AD Tenant: " + tenant);
Console.WriteLine("Azure AD Client ID: " + clientId);
Console.WriteLine("Azure AD Authority: " + authority);

//URL of your Azure DevOps account.
string azureDevOpsOrganizationUrl = configuration["ado:OrganizationUrl"];

if(!azureDevOpsOrganizationUrl.EndsWith("/"))
{
  azureDevOpsOrganizationUrl += "/";
}

string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

Console.WriteLine("Azure DevOps Organization URL: " + azureDevOpsOrganizationUrl);
Console.WriteLine("AzDO Scope: " + scopes[0]);

// // MSAL Public client app
IPublicClientApplication application;

try
  {
    var authResult = await SignInUserAndGetTokenUsingMSAL(scopes);

    // Create authorization header of the form "Bearer {AccessToken}"
    var authHeader = authResult.CreateAuthorizationHeader();

    ListProjects(authHeader);
  }
  catch (Exception ex)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Something went wrong.");
    Console.WriteLine("Message: " + ex.Message + "\n");
  }

/// <summary>
/// Sign-in user using MSAL and obtain an access token for Azure DevOps
/// </summary>
/// <param name="scopes"></param>
/// <returns>AuthenticationResult</returns>
async Task<AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes)
{
  // Initialize the MSAL library by building a public client application
  application = PublicClientApplicationBuilder.Create(clientId)
                             .WithAuthority(authority)
                             .WithDefaultRedirectUri()
                             .Build();

  AuthenticationResult result;

  try
  {
    var accounts = await application.GetAccountsAsync();
    result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
            .ExecuteAsync();
  }
  catch (MsalUiRequiredException ex)
  {
    // If the token has expired, prompt the user with a login prompt
    result = await application.AcquireTokenInteractive(scopes)
            .WithClaims(ex.Claims)
            .ExecuteAsync();
  }
  return result;
}

/// <summary>
/// Get all projects in the organization that the authenticated user has access to and print the results.
/// </summary>
/// <param name="authHeader"></param>
void ListProjects(string authHeader)
{
  // use the httpclient
  using (var client = new HttpClient())
  {
    client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
    client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
    client.DefaultRequestHeaders.Add("Authorization", authHeader);

    // connect to the REST endpoint
    HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

    Console.WriteLine("Response: " + response.RequestMessage.RequestUri);

    // check to see if we have a succesfull respond
    if (response.IsSuccessStatusCode)
    {
      Console.WriteLine("Succesful REST call");
      var result = response.Content.ReadAsStringAsync().Result;
      Console.WriteLine(result);
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
      throw new UnauthorizedAccessException();
    }
    else
    {
      Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
    }
  }
}