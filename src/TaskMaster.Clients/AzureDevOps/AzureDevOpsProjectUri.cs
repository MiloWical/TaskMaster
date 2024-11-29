namespace TaskMaster.Clients.AzureDevOps;

public class AzureDevOpsProjectUri(string projectUrl)
{
  public Uri ProjectUri => _projectUri; 

  private readonly Uri _projectUri = projectUrl.EndsWith('/')
          ? new Uri(projectUrl)
          : new Uri($"{projectUrl}/");
}
