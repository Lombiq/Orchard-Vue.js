using Newtonsoft.Json.Linq;

namespace Lombiq.VueJs.Models;

public class VueComponentApp(JObject viewModel)
{
    public string Id { get; set; }
    public string Class { get; set; }
    public string ComponentName { get; set; }
    public string ModelProperty { get; set; } = "value";
    public JObject ViewModel { get; } = viewModel;
}
