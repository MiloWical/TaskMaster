using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Clients.AzureDevOps;

namespace TaskMaster.DependencyInjection;

public static class ClientExtensions
{
  public static TaskMasterBuilder WithAzureDevopsProjectUri(this TaskMasterBuilder taskMasterBuilder, string projectUri)
  {
    taskMasterBuilder.ServiceCollection.AddSingleton(new AzureDevOpsProjectUri(projectUri));
    return taskMasterBuilder;
  }
  public static TaskMasterBuilder UsingAzureDevOpsClient(this TaskMasterBuilder taskMasterBuilder)
  {
    taskMasterBuilder.ServiceCollection.AddSingleton<IAzureDevOpsClient, AzureDevOpsClient>();
    return taskMasterBuilder;
  }

   public static TaskMasterBuilder UsingAzureDevOpsConsoleClient(this TaskMasterBuilder taskMasterBuilder)
  {
    taskMasterBuilder.ServiceCollection.AddSingleton<IAzureDevOpsClient, AzureDevOpsConsoleClient>();
    return taskMasterBuilder;
  }
}
