using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Lombiq.VueJs.Models;

internal record VueComponentApp(
    string Id,
    string Class,
    string ComponentName,
    IEnumerable<string> ModelProperties,
    JObject ViewModel);
