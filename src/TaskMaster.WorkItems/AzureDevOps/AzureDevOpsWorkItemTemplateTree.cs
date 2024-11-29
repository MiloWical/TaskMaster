namespace TaskMaster.WorkItems.AzureDevOps;

public class AzureDevOpsWorkItemTemplateTreeNode
{
    public string InstanceId { get; init; } = Guid.NewGuid().ToString();
    private readonly string _workItemTemplateId = null!;
    public string WorkItemTemplateId
    {
      get => _workItemTemplateId;
      init => _workItemTemplateId = value;
    }

    public bool HasWorkItemTemplate => !string.IsNullOrWhiteSpace(WorkItemTemplateId);

    public IEnumerable<string> ChildInstanceIds { get; init; }

    public bool HasChildren => ChildInstanceIds.Any();

    public AzureDevOpsWorkItemTemplateTreeNode()
      : this(string.Empty, [])
    {}

    public AzureDevOpsWorkItemTemplateTreeNode(string workItemTemplateId, params string[] children)
    {
        WorkItemTemplateId = workItemTemplateId;
        ChildInstanceIds = children;
    }

    public AzureDevOpsWorkItemTemplateTreeNode(string workItemTemplateId)
      : this(workItemTemplateId, [])
    {}

    public AzureDevOpsWorkItemTemplateTreeNode(params string[] children)
      : this(string.Empty, children)
    {}
}
