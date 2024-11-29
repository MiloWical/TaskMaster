using System.Text.Json;
using System.Text.Json.Nodes;
using TaskMaster.WorkItems.AzureDevOps;

namespace TaskMaster.Clients.AzureDevOps;

public class AzureDevOpsConsoleClient : IAzureDevOpsClient
{
  private static JsonSerializerOptions _jsonSerializerOptions = new()
  {
    WriteIndented = true,
  };

  private static int _id = 1000;

  public JsonNode CreateWorkItem(AzureDevOpsWorkItem workItem, IDictionary<string, string> templateValues)
  {
    var result = workItem.ToPatchDto(templateValues);

    // Console.WriteLine(result.ToJsonString(_jsonSerializerOptions));

    return new JsonObject {
      ["id"] = "work-item:"+_id++,
    };
  }

  public JsonNode CreateWorkItemLink(AzureDevOpsWorkItemLink workItemLink, string source, string target, IDictionary<string, string> templateValues)
  {
    var result = workItemLink.ToPatchDto(target, templateValues);
    // Console.WriteLine(result.ToJsonString(_jsonSerializerOptions));

    return new JsonObject {
      ["id"] = "link:"+_id++,
    };
  }

    public JsonNode GetWorkItem(int workItemId)
    {
        throw new NotImplementedException();
    }

    public JsonNode ListWorkItemRelationTypes()
    {
        throw new NotImplementedException();
    }

    public JsonNode ListWorkItemTypes()
    {
        throw new NotImplementedException();
    }
}
