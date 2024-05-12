using System.Text.Json.Serialization;

namespace Lombiq.VueJs.Models;

public class UrlAndText
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
