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
    private const string Root = $"~/{Area}/";
    private const string Js = Root + "js/";
    private const string Vendors = Root + "vendors/";

    private const string VueCdnRoot = $"https://unpkg.com/vue@{VueVersion}/dist/";
    private const string VueRouterCdnRoot = $"https://unpkg.com/vue-router@{VueRouterVersion}/dist/";

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest
            .DefineScriptModule(Vue3)
            .SetUrl(Vendors + "vue/vue.esm-browser.prod.js", Vendors + "vue/vue.esm-browser.js")
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(VueVersion);

        _manifest
            .DefineScriptModule(VueRouter)
            .SetUrl(Vendors + "vue-router/vue-router.esm-browser.prod.js", Vendors + "vue-router/vue-router.esm-browser.js")
            .SetCdn(VueRouterCdnRoot + "vue-router.esm-browser.prod.js", VueCdnRoot + "vue-router.esm-browser.js")
            .SetVersion(VueRouterVersion);

#if DEBUG
        // This is only required for the vue-router in development mode. We need to use the CDN version because the
        // NPM version is not compiled and should be used with a bundler.
        _manifest
            .DefineScriptModule("@vue/devtools-api")
            .SetCdn("https://unpkg.com/@vue/devtools-api@6.2.1/lib/esm/index.js")
            .SetVersion("6.2.1");
#endif

        _manifest
            .DefineScriptModule(VueComponentApp)
            .SetAttribute("defer", string.Empty)
            .SetUrl(Js + "vue-component-app.min.mjs", Js + "vue-component-app.mjs");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
