using Newtonsoft.Json;

namespace Lombiq.VueJs.Helpers;

public class VueServerValuesHelper
{
    public static string InitializeContainer(string name, string values) =>
        "if (!window.Vue.$orchardCore) window.Vue.$orchardCore = {};" +
        $"window.Vue.$orchardCore[{JsonConvert.SerializeObject(name)}] = {JsonConvert.SerializeObject(values)};";
}
