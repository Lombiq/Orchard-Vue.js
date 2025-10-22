using Lombiq.HelpfulLibraries.Attributes;
using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.VueJs.Resources.Constants;
using static Lombiq.VueJs.Resources.Constants.ResourceNames;

namespace Lombiq.VueJs.Resources;

[ConstantFromJson("VueVersion", "package.json", "vue")]
[ConstantFromJson("VueRouterVersion", "package.json", "vue-router")]
public partial class ResourceManagementOptionsConfiguration : ResourceManagementOptionsConfiguratorBase
{
    private const string VueCdnRoot = $"https://unpkg.com/vue@{VueVersion}/dist/";
    private const string VueRouterCdnRoot = $"https://unpkg.com/vue-router@{VueRouterVersion}/dist/";

    protected override string Area => FeatureIds.Area;

    protected override void Configure(ResourceManagementContext context)
    {
        context.DefineVendorScriptModule(
                Vue3,
                (Production: "vue/vue.esm-browser.prod.js", Debug: "vue/vue.esm-browser.js"))
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(VueVersion);

        // Using only the prod version of Vue Router because the dev version requires @vue/devtools-api which is not
        // available as an NPM package only on the CDN.
        context.DefineVendorScriptModule(VueRouter, "vue-router/vue-router.esm-browser.prod.js", Vue3)
            .SetCdn(VueRouterCdnRoot + "vue-router.esm-browser.prod.js")
            .SetVersion(VueRouterVersion);
    }
}
