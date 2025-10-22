using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Constants.FeatureIds;
using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs;

[ConstantFromJson("VueVersion", "package.json", "vue")]
[ConstantFromJson("VueRouterVersion", "package.json", "vue-router")]
public partial class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string Js = $"~/{Area}/js/";

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration() =>
        _manifest
            .DefineScriptModule(VueComponentApp)
            .SetAttribute("defer", string.Empty)
            .SetUrl(Js + "vue-component-app.min.mjs", Js + "vue-component-app.mjs");

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
