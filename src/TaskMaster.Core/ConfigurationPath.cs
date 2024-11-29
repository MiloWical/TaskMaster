namespace TaskMaster.Core;

internal static class ConfigurationPath
{
  public const string AzureDevOps = "AzureDevOps";
  public const string AzureDevOpsOrganizationUrl = AzureDevOps + ":OrganizationUrl";
  public const string AzureDevOpsProject = AzureDevOps + ":Project";
  public const string Authentication = "Authentication";
  public const string Msal = Authentication + ":Msal";
  public const string MsalClientId = Msal + ":ClientId";
  public const string MsalAuthority = Msal + ":Msal:Authority";
}
