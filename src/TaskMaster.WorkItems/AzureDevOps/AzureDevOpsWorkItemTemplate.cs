namespace TaskMaster.WorkItems.AzureDevOps;

public readonly struct AzureDevOpsWorkItemTemplate
{
  public AzureDevOpsWorkItemTemplate()
  {
  }

  public string Id { get; init; } = Guid.NewGuid().ToString();
  public AzureDevOpsWorkItem WorkItem { get; init; }
}
