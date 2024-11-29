using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.DependencyInjection;

namespace TaskMaster.Core;

public class TaskMaster(IConfiguration configuration)
{
  private IServiceProvider _serviceProvider = AddTaskMaster(configuration);

  private static ServiceProvider AddTaskMaster(IConfiguration configuration)
  {
    var services = new ServiceCollection();

    var builder = services.AddTaskMaster();

    AddClients(builder, configuration);
    AddAuthentication(builder, configuration);

    return services.BuildServiceProvider();
  }

  private static TaskMasterBuilder AddClients(TaskMasterBuilder builder, IConfiguration configuration)
  {
    TryAddAzureDevOps(builder, configuration);

    // "TryAdd" other mechanisms here...

    return builder;
  }

  private static void TryAddAzureDevOps(TaskMasterBuilder builder, IConfiguration configuration)
  {
    var azureDevOps = configuration.GetSection(ConfigurationPath.AzureDevOps);

    if (azureDevOps.Exists())
    {
      builder.UsingAzureDevOpsClient()
             .WithAzureDevopsProjectUri(string.Format(
                configuration[ConfigurationPath.AzureDevOpsOrganizationUrl]!,
                configuration[ConfigurationPath.AzureDevOpsProject]!))
             .UsingAzureDevOpsLoader();
    }
  }

  private static void AddAuthentication(TaskMasterBuilder builder, IConfiguration configuration)
  {
    var authentication = configuration.GetSection(ConfigurationPath.Authentication);

    if (authentication.Exists())
    {
      TryAddMsal(builder, configuration);
    }

    // "TryAdd" other mechanisms here...
  }

  private static void TryAddMsal(TaskMasterBuilder builder, IConfiguration configuration)
  {
    var msal = configuration.GetSection(ConfigurationPath.Msal);

    if (msal.Exists())
    {
      builder.WithMsal(msal[ConfigurationPath.MsalClientId]!, msal[ConfigurationPath.MsalAuthority]!);
    }
  }
}
