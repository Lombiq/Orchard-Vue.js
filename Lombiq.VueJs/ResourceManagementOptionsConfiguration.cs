using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Constants.FeatureIds;
using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string Root = "~/" + Area;

    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration() => _manifest
            .DefineScript(VueComponentApp)
            .SetUrl(Root + "/vue-component-app.min.js", Root + "/vue-component-app.js");

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
