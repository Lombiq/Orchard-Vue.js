using Newtonsoft.Json.Linq;

namespace Lombiq.VueJs.Models;

public class VueComponentApp
{
    public string Id { get; set; }
    public string Class { get; set; }
    public string ComponentName { get; set; }
    public string ModelProperty { get; set; } = "value";
    public JObject ViewModel { get; }

    public VueComponentApp(JObject viewModel) => ViewModel = viewModel;
}
