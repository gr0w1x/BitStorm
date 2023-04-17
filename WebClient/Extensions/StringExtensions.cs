using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebClient.Extensions;

public static class StringExtensions
{
    public static string ToQueryString(this object obj, string? prefix = null)
        => ParseJObject(JsonConvert.SerializeObject(obj), prefix);

    private static string ParseJObject(string json, string? prefix)
    {
        var jo = (JObject)JsonConvert.DeserializeObject(json)!;
        var tokens = new List<string>();
        SearchAndParseToken(jo, prefix, tokens);
        return string.Join("&", tokens);
    }

    private static void SearchAndParseToken(JToken jt, string? key, ICollection<string> tokens)
    {
        if (jt is JArray)
        {
            int i = 0;
            foreach (var ji in jt)
            {
                string indexed = $"{key}[{i++}]";
                SearchAndParseToken(ji, indexed, tokens);
            }
        }
        else if (jt is JObject jo)
        {
            var keyFormatted = string.IsNullOrWhiteSpace(key) ? string.Empty : $"{key}.";
            foreach (var jop in jo.Children().Cast<JProperty>())
            {
                string named = $"{keyFormatted}{jop.Name}";
                SearchAndParseToken(jop.Value, named, tokens);
            }
        }
        else
        {
            var toString = jt?.ToString();
            if (!string.IsNullOrWhiteSpace(toString))
            {
                tokens.Add($"{key}={HttpUtility.UrlEncode(toString)}");
            }
        }
    }
}
