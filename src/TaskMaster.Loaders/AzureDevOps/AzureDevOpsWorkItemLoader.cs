using TaskMaster.Clients.AzureDevOps;
using TaskMaster.WorkItems.AzureDevOps;

namespace TaskMaster.Loaders.AzureDevOps;

public class AzureDevOpsWorkItemLoader
{
  private readonly IAzureDevOpsClient _azureDevOpsClient;

  public AzureDevOpsWorkItemLoader(IAzureDevOpsClient azureDevOpsClient)
  {
    _azureDevOpsClient = azureDevOpsClient;
  }

  public void LoadWorkItems(AzureDevOpsWorkItemScaffold scaffold, IDictionary<string, string> templateValues)
  {
    var workItemMap = BuildWorkItems(scaffold, templateValues);
    BuildWorkItemLinks(workItemMap, scaffold, templateValues);
  }

  private Dictionary<string, string> BuildWorkItems(AzureDevOpsWorkItemScaffold scaffold, IDictionary<string, string> templateValues)
  {
    var workItemIds = new Dictionary<string, string>();
    var root = scaffold.WorkItemTree.First(treeNode => treeNode.InstanceId == scaffold.RootInstanceId);
    var workItemId = BuildWorkItemFromTemplate(root.WorkItemTemplateId, scaffold, templateValues);
    Console.WriteLine($"Work Item ID: {workItemId}; Instance ID: {root.InstanceId}");
    workItemIds.Add(root.InstanceId, workItemId);

    TraverseTree(root, workItemIds, scaffold, templateValues);

    return workItemIds;
  }

  private void BuildWorkItemLinks(IDictionary<string, string> workItemIds, AzureDevOpsWorkItemScaffold scaffold, IDictionary<string, string> templateValues)
  {
    foreach (var link in scaffold.WorkItemLinks)
    {
      var sourceWorkItem = workItemIds[link.SourceInstanceId];
      var targetWorkItem = workItemIds[link.TargetInstanceId];
      var linkResult = _azureDevOpsClient.CreateWorkItemLink(link, sourceWorkItem, targetWorkItem, templateValues);

      Console.WriteLine($"{linkResult["id"]}: {sourceWorkItem}[{link.SourceInstanceId}] ==[{link.LinkType}]==> {targetWorkItem}[{link.TargetInstanceId}]");
    }
  }

  private void TraverseTree(AzureDevOpsWorkItemTemplateTreeNode node, Dictionary<string, string> workItemIds, AzureDevOpsWorkItemScaffold scaffold, IDictionary<string, string> templateValues)
  {
    if (!node.HasChildren)
    {
      return;
    }

    // Get child nodes first, then process them

    var children = scaffold.WorkItemTree.Where(x => node.ChildInstanceIds.Contains(x.InstanceId));

    foreach (var child in children)
    {
      var workItemId = BuildWorkItemFromTemplate(child.WorkItemTemplateId, scaffold, templateValues);
      workItemIds.Add(child.InstanceId, workItemId);
      Console.WriteLine($"Work Item ID: {workItemId}; Instance ID: {child.InstanceId}");
    }

    foreach (var child in children)
    {
      TraverseTree(child, workItemIds, scaffold, templateValues);
    }
  }

  private string BuildWorkItemFromTemplate(string templateId, AzureDevOpsWorkItemScaffold scaffold, IDictionary<string, string> templateValues)
  {
    var template = scaffold.WorkItemTemplates.First(x => x.Id == templateId);
    var id = _azureDevOpsClient.CreateWorkItem(template.WorkItem, templateValues)["id"]!.ToString();

    return id;
  }
}
