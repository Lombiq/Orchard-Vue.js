using Lombiq.HelpfulLibraries.SourceGenerators;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Constants.FeatureIds;
using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs;

[ConstantFromJson("VueVersion", "package.json", "vue")]
public partial class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string Root = $"~/{Area}/";
    private const string Js = Root + "js/";
    private const string Vendors = Root + "vendors/";

    private const string VueCdnRoot = $"https://unpkg.com/vue@{VueVersion}/dist/";

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest
            .DefineScriptModule(Vue3)
            .SetUrl(Vendors + "vue/vue.esm-browser.prod.js", Vendors + "vue/vue.esm-browser.js")
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(VueVersion);

        _manifest
            .DefineScriptModule(VueComponentApp)
            .SetAttribute("defer", string.Empty)
            .SetUrl(Js + "vue-component-app.min.mjs", Js + "vue-component-app.mjs");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
