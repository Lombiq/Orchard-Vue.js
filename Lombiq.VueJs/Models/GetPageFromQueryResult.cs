using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lombiq.VueJs.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public class GetPageFromQueryResult
{
    public IEnumerable<object> Items { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
}
