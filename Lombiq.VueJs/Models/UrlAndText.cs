using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lombiq.VueJs.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class UrlAndText
{
    public string Url { get; set; }
    public string Text { get; set; }
}
