using System.Text;
using System.Text.RegularExpressions;

namespace TaskMaster.WorkItems;

public static partial class TemplateExtensions
{
  [GeneratedRegex(@"{{(.*?)}}", RegexOptions.Compiled)]
  private static partial Regex MyRegex();

  internal static readonly Regex FieldRegex = MyRegex();
  public static IEnumerable<string> ReadTemplatedFields(this string templateEntry)
  {
    var fields = new HashSet<string>();
    var matches = FieldRegex.Matches(templateEntry);

    return matches.Select(m => m.Value).ToArray();
  }

  public static void AddAll<T>(this HashSet<T> set, IEnumerable<T> values)
  {
    foreach (var value in values)
    {
      set.Add(value);
    }
  }

  public static string ReplaceTemplateValues(this string template, IDictionary<string, string> templateValues)
  {
    var result = new StringBuilder(template);
    foreach (var (key, value) in templateValues)
    {
      result = result.Replace(key, value);
    }

    return result.ToString();
  }
}
