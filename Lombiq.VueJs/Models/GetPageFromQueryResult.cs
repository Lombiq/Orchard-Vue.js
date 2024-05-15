using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lombiq.VueJs.Models;

public class GetPageFromQueryResult
{
    [JsonPropertyName("items")]
    public IEnumerable<object> Items { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }
}
