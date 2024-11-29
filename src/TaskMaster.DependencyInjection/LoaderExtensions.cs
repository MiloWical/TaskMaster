using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Loaders.AzureDevOps;

namespace TaskMaster.DependencyInjection;

public static class LoaderExtensions
{
  public static TaskMasterBuilder UsingAzureDevOpsLoader(this TaskMasterBuilder taskMasterBuilder)
  {
    taskMasterBuilder.ServiceCollection.AddSingleton<AzureDevOpsWorkItemLoader>();
    return taskMasterBuilder;
  }
}
