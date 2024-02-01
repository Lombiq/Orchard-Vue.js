using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Constants.FeatureIds;
using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string Root = $"~/{Area}/";
    private const string Js = Root + "js/";
    private const string Vendors = Root + "vendors/";

    private const string VueVersion = "3.4.15";
    private const string VueCdnRoot = $"https://unpkg.com/vue@{VueVersion}/dist/";

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest
            .DefineScript(Vue3)
            .SetUrl(Vendors + "vue/vue.esm-browser.prod.js", Vendors + "vue/vue.esm-browser.js")
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(VueVersion);

        _manifest
            .DefineScript(VueComponentApp)
            .SetUrl(Js + "vue-component-app.min.js", Js + "/vue-component-app.js");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
