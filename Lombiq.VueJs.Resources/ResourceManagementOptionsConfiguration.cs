using Lombiq.HelpfulLibraries.Attributes;
using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.VueJs.Resources.Constants;
using System;
using static Lombiq.VueJs.Resources.Constants.ResourceNames;

namespace Lombiq.VueJs.Resources;

[LibManVersions]
public partial class ResourceManagementOptionsConfiguration : ResourceManagementOptionsConfiguratorBase
{
    [Obsolete($"Use the values in {nameof(LibManVersions)}.")]
    public const string VueVersion = LibManVersions.Vue;

    [Obsolete($"Use the values in {nameof(LibManVersions)}.")]
    public const string VueRouterVersion = LibManVersions.VueRouter;

    private const string VueCdnRoot = $"https://unpkg.com/vue@{LibManVersions.Vue}/dist/";
    private const string VueRouterCdnRoot = $"https://unpkg.com/vue-router@{LibManVersions.VueRouter}/dist/";

    protected override string Area => FeatureIds.Area;

    protected override void Configure(ResourceManagementContext context)
    {
        context.DefineScriptModule(VueDevtoolsApi, "dummy-vue-devtools-api.mjs");

        context.DefineVendorScriptModule(
                Vue3,
                (Production: "vue/dist/vue.esm-browser.prod.js", Debug: "vue/dist/vue.esm-browser.js"))
            .SetCdn(VueCdnRoot + "vue.esm-browser.prod.js", VueCdnRoot + "vue.esm-browser.js")
            .SetVersion(LibManVersions.Vue);

        context.DefineVendorScriptModule(VueRouter, "vue-router/dist/vue-router.esm-browser.prod.js", Vue3, VueDevtoolsApi)
            .SetCdn(VueRouterCdnRoot + "vue-router.esm-browser.prod.js", VueRouterCdnRoot + "vue-router.esm-browser.js")
            .SetVersion(LibManVersions.VueRouter);

        context.DefineScript(SetupEnvironment, "setup-environment.js");
    }
}
