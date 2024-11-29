// https://learn.microsoft.com/en-us/azure/devops/boards/queries/link-type-reference?view=azure-devops#work-link-type
using System.Text.Json.Nodes;

namespace TaskMaster.WorkItems.AzureDevOps;

public readonly struct AzureDevOpsWorkItemLink
{
    public AzureDevOpsWorkItemLink()
    {
    }

    public required string ProjectUrl { get; init; }
    public required string SourceInstanceId { get; init; }
    public required string TargetInstanceId { get; init; }
    public required AzureDevOpsWorkItemLinkType LinkType { get; init; }

    public string LinkComment { get; init; } = string.Empty;

    public readonly JsonNode ToPatchDto(string target, IDictionary<string, string> templateValues)
    {
        var patchDocument = new JsonArray() 
        {
            new JsonObject()
            {
                ["op"] = "add",
                ["path"] = "/relations/-",
                ["value"] = new JsonObject()
                {
                    ["rel"] = ConvertLinkTypeToString(LinkType),
                    ["url"] = $"{ProjectUrl}/_apis/wit/workItems/{target}"
                }
            }
        };

        if (!string.IsNullOrEmpty(LinkComment))
        {
            patchDocument[0]!["value"]!.AsObject().Add("attributes", new JsonObject()
            {
                ["comment"] = LinkComment.ReplaceTemplateValues(templateValues),
            });
        }
        return patchDocument;
    }

    private static string ConvertLinkTypeToString(AzureDevOpsWorkItemLinkType linkType)
    {
        return linkType switch
        {
            AzureDevOpsWorkItemLinkType.Parent => "System.LinkTypes.Hierarchy-Reverse",
            AzureDevOpsWorkItemLinkType.Child => "System.LinkTypes.Hierarchy-Forward",
            AzureDevOpsWorkItemLinkType.Related => "System.LinkTypes.Related",
            AzureDevOpsWorkItemLinkType.Successor => "System.LinkTypes.Dependency-Forward",
            AzureDevOpsWorkItemLinkType.Predecessor => "System.LinkTypes.Dependency-Reverse",
            AzureDevOpsWorkItemLinkType.Affects => "Microsoft.VSTS.Common.Affects-Forward",
            AzureDevOpsWorkItemLinkType.AffectedBy => "Microsoft.VSTS.Common.Affects-Reverse",
            _ => throw new ArgumentOutOfRangeException(nameof(linkType), linkType, null)
        };
    }
}
