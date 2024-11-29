using System.Text.Json.Nodes;
using TaskMaster.WorkItems.AzureDevOps;

namespace TaskMaster.Clients.AzureDevOps;

public interface IAzureDevOpsClient
{
    JsonNode GetWorkItem(int workItemId);
    JsonNode ListWorkItemRelationTypes();
    JsonNode ListWorkItemTypes();
    JsonNode CreateWorkItem(AzureDevOpsWorkItem workItem, IDictionary<string, string> templateValues);
    JsonNode CreateWorkItemLink(AzureDevOpsWorkItemLink link, string sourceWorkItemId, string targetWorkItemId, IDictionary<string, string> templateValues);
    
}
