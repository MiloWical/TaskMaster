Basic usage:
```csharp
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Clients.AzureDevOps;
using TaskMaster.DependencyInjection;
using TaskMaster.Loaders.AzureDevOps;
using TaskMaster.WorkItems.AzureDevOps;

var serializerOptions = new JsonSerializerOptions()
{
  WriteIndented = true,
  Converters =
    {
        new JsonStringEnumConverter()
    }
};

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

var configuration = builder.Build();
string aadInstance = configuration["msal:AADInstance"]!;
string tenant = configuration["msal:Tenant"]!;
string clientId = configuration["msal:ClientId"]!;
string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

Console.WriteLine("Azure AD Instance: " + aadInstance);
Console.WriteLine("Azure AD Tenant: " + tenant);
Console.WriteLine("Azure AD Client ID: " + clientId);
Console.WriteLine("Azure AD Authority: " + authority);

var scaffold = JsonSerializer.Deserialize<AzureDevOpsWorkItemScaffold>(File.OpenRead("scaffold.json"), serializerOptions)!;



Dictionary<string, string> templateValues = [];

foreach (var templateField in scaffold.TemplatedFields)
{
  Console.Write(templateField + ": ");

  var value = Console.ReadLine()!;

  templateValues.Add(templateField, value);
}


// var client = new AzureDevOpsConsoleClient();

// var loader = new AzureDevOpsWorkItemLoader(client);

// loader.LoadWorkItems(scaffold, templateValues);

var serviceCollection = new ServiceCollection();

serviceCollection.AddTaskMaster()
  .UsingAzureDevOpsClient()
  .WithAzureDevopsProjectUri(string.Format(configuration["azdo:OrganizationUrl"]!, configuration["azdo:Project"]!))
  .WithMsal(clientId, authority)
  .UsingAzureDevOpsLoader();

var serviceProvider = serviceCollection.BuildServiceProvider();

var loader = serviceProvider.GetRequiredService<AzureDevOpsWorkItemLoader>();
loader.LoadWorkItems(scaffold, templateValues);
```