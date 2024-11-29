namespace TaskMaster.WorkItems.AzureDevOps;

public class AzureDevOpsWorkItemScaffold(IEnumerable<AzureDevOpsWorkItemTemplate> workItemTemplates, IEnumerable<AzureDevOpsWorkItemTemplateTreeNode> workItemTree, IEnumerable<AzureDevOpsWorkItemLink> workItemLinks)
{
    public IEnumerable<AzureDevOpsWorkItemTemplate> WorkItemTemplates { get; init; } = workItemTemplates;
    public IEnumerable<AzureDevOpsWorkItemTemplateTreeNode> WorkItemTree { get; init; } = workItemTree;
    public IEnumerable<AzureDevOpsWorkItemLink> WorkItemLinks { get; init; } = workItemLinks;
    public required string RootInstanceId { get; init; }
    
    private readonly HashSet<string> _templatedFields = [];
    private bool templatedFieldsRead = false;
    public ISet<string> TemplatedFields { 
      get
      {
        if (!templatedFieldsRead)
        {
          foreach (var template in WorkItemTemplates)
          {
            _templatedFields.AddAll(template.WorkItem.TemplatedFields);
          }

          templatedFieldsRead = true;
        }

        return _templatedFields;
      }
    }
}
