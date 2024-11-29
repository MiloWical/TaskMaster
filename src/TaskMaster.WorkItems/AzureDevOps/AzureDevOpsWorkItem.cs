using System.Text.Json.Nodes;

namespace TaskMaster.WorkItems.AzureDevOps;

public struct AzureDevOpsWorkItem
{
  public required string Title { get; init; }
  public required AzureDevOpsWorkItemType Type { get; init; }
  public required string TeamProject { get; init; }
  public required string AreaPath { get; init; }
  public required string IterationPath { get; init; }
  public required string AssignedTo { get; init; }
  public required string Description { get; init; }
  public readonly IList<string> Fields { get; }

  private HashSet<string> _templatedFields;
  private bool templatedFieldsRead = false;
  public ISet<string> TemplatedFields
  {
    get
    {
      if (!templatedFieldsRead)
      {
        _templatedFields.AddAll(Title.ReadTemplatedFields());
        _templatedFields.AddAll(TeamProject.ReadTemplatedFields());
        _templatedFields.AddAll(AreaPath.ReadTemplatedFields());
        _templatedFields.AddAll(IterationPath.ReadTemplatedFields());
        _templatedFields.AddAll(AssignedTo.ReadTemplatedFields());
        _templatedFields.AddAll(Description.ReadTemplatedFields());

        foreach (var field in Fields)
        {
          _templatedFields.AddAll(field.ReadTemplatedFields());
        }

        templatedFieldsRead = true;
      }

      return _templatedFields;
    }
  }

  public AzureDevOpsWorkItem()
  {
    Fields = [];
    _templatedFields = [];
  }

  public AzureDevOpsWorkItem(string title, AzureDevOpsWorkItemType type, string teamProject, string areaPath, string iterationPath, string assignedTo, string description)
    : this()
  {
    Title = title;
    Type = type;
    TeamProject = teamProject;
    AreaPath = areaPath;
    IterationPath = iterationPath;
    AssignedTo = assignedTo;
    Description = description;
  }

  public JsonNode ToPatchDto(IDictionary<string, string> templateValues)
  {
    var patchDocument = new JsonArray() {
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.Title",
        ["value"] = Title.ReplaceTemplateValues(templateValues)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.WorkItemType",
        ["value"] = ConvertWorkItemTypeToString(Type)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.TeamProject",
        ["value"] = TeamProject.ReplaceTemplateValues(templateValues)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.AreaPath",
        ["value"] = AreaPath.ReplaceTemplateValues(templateValues)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.IterationPath",
        ["value"] = IterationPath.ReplaceTemplateValues(templateValues)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.AssignedTo",
        ["value"] = AssignedTo.ReplaceTemplateValues(templateValues)
      },
      new JsonObject()
      {
        ["op"] = "add",
        ["path"] = "/fields/System.Description",
        ["value"] = Description.ReplaceTemplateValues(templateValues)
      },
    };

    foreach (var field in Fields)
    {
      var fieldParts = field.Split('=');
      patchDocument.Add(new JsonObject()
      {
        ["op"] = "add",
        ["path"] = $"/fields/{fieldParts[0].ReplaceTemplateValues(templateValues)}",
        ["value"] = fieldParts[1].ReplaceTemplateValues(templateValues)
      });
    }

    return patchDocument;
  }

  public static string ConvertWorkItemTypeToString(AzureDevOpsWorkItemType workItemType)
  {
    return workItemType switch
    {
      AzureDevOpsWorkItemType.Epic => "Epic",
      AzureDevOpsWorkItemType.Feature => "Feature",
      AzureDevOpsWorkItemType.ProductBacklogItem => "Product Backlog Item",
      AzureDevOpsWorkItemType.Task => "Task",
      _ => throw new ArgumentOutOfRangeException(nameof(workItemType), workItemType, null)
    };
  }
}
