using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Lombiq.VueJs.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class GetPageFromQueryResult
{
    public IEnumerable<object> Items { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
}
