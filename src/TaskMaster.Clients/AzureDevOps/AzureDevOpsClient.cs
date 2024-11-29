// https://learn.microsoft.com/en-us/rest/api/azure/devops/wit/work-item-relation-types/list?view=azure-devops-rest-7.1&tabs=HTTP

using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using TaskMaster.Authentication;
using TaskMaster.WorkItems.AzureDevOps;

namespace TaskMaster.Clients.AzureDevOps;

public class AzureDevOpsClient : IAzureDevOpsClient
{
  private readonly Lazy<HttpClient> _httpClient;

  public AzureDevOpsClient(IAuthenticationProvider authenticationProvider, AzureDevOpsProjectUri azureDevOpsProjectUri)
  {
    _httpClient = new Lazy<HttpClient>(() =>
    {
      var credential = authenticationProvider.LoginAsync().Result;

      var client = new HttpClient
      {
        BaseAddress = azureDevOpsProjectUri.ProjectUri
      };

      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
      client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
      client.DefaultRequestHeaders.Add("Authorization", credential.Value);

      return client;
    });
  }

  public JsonNode GetWorkItem(int workItemId)
  {
    var response = SendRequest(HttpMethod.Get, $"_apis/wit/workitems/{workItemId}?api-version=7.1&$expand=all");

    return response;
  }

  public JsonNode ListWorkItemRelationTypes()
  {
    var response = SendRequest(HttpMethod.Get, "_apis/wit/workitemrelationtypes?api-version=7.1");

    return response;
  }

  public JsonNode ListWorkItemTypes()
  {
    var response = SendRequest(HttpMethod.Get, "_apis/wit/workitemtypes?api-version=7.1");

    return response;
  }

  public JsonNode CreateWorkItem(AzureDevOpsWorkItem workItem, IDictionary<string, string> templateValues)
  {
    var workItemType = UrlEncoder.Default.Encode(AzureDevOpsWorkItem.ConvertWorkItemTypeToString(workItem.Type));

    var url = $"_apis/wit/workitems/${workItemType}?$expand=all&api-version=7.1";
    var response = SendRequest(HttpMethod.Post, url, workItem.ToPatchDto(templateValues), "application/json-patch+json");

    return response;
  }

  public JsonNode CreateWorkItemLink(AzureDevOpsWorkItemLink workItemLink, string source, string target, IDictionary<string, string> templateValues)
  {
    var url = $"_apis/wit/workitems/{source}?api-version=7.1&$expand=all";
    var response = SendRequest(HttpMethod.Patch, url, workItemLink.ToPatchDto(target, templateValues), "application/json-patch+json");

    return response;
  }

  private JsonNode SendRequest(HttpMethod httpMethod, string url, JsonNode content = null!, string mediaTypeHeader = "application/json")
  {
    var request = new HttpRequestMessage(httpMethod, url);

    if (content != null)
    {
      request.Content = new StringContent(content.ToJsonString());
      request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaTypeHeader);
    }

    var response = _httpClient.Value.Send(request);

    return ProcessResponse(response);
  }

  private static JsonNode ProcessResponse(HttpResponseMessage response)
  {
    if (response.IsSuccessStatusCode)
    {
      var result = response.Content.ReadAsStringAsync().Result;
      return JsonNode.Parse(result)!;
    }

    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
      throw new UnauthorizedAccessException();
    }

    throw new Exception($"{response.StatusCode}:{response.ReasonPhrase}");
  }


}