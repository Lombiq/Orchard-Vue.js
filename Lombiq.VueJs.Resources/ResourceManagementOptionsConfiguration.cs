using Lombiq.HelpfulLibraries.Attributes;
using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.VueJs.Resources.Constants;
using static Lombiq.VueJs.Resources.Constants.ResourceNames;

namespace Lombiq.VueJs.Resources;

[LibManVersions]
public partial class ResourceManagementOptionsConfiguration : ResourceManagementOptionsConfiguratorBase
{
    private const string VueCdnRoot = $"https://unpkg.com/vue@{LibManVersions.Vue}/dist/";

    protected override string Area => FeatureIds.Area;

    protected override void Configure(ResourceManagementContext context)
    {
        context.DefineScriptModule(VueDevtoolsApi, "dummy-vue-devtools-api.mjs");

        context.DefineVendorScriptModule(
                Vue3,
                (Production: "vue/dist/vue.esm-browser.prod.js", Debug: "vue/dist/vue.esm-browser.js"))
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(LibManVersions.Vue);

        context.DefineVendorScriptModule(VueRouter, "vue-router/dist/vue-router.mjs", Vue3, VueDevtoolsApi)
            .SetCdn($"https://unpkg.com/vue-router@{LibManVersions.VueRouter}/dist/vue-router.mjs")
            .SetVersion(LibManVersions.VueRouter);

        context.DefineScript(SetupEnvironment, "setup-environment.js");
    }
}
