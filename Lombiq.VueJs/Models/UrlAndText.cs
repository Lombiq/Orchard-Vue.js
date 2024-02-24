using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lombiq.VueJs.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class UrlAndText
{
    public string Url { get; set; }
    public string Text { get; set; }
}
