using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Samples.Constants.ResourceNames;
using static Lombiq.VueJs.Samples.Constants.FeatureIds;

namespace Lombiq.VueJs.Samples
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private const string Root = "~/" + Area;

        private static readonly ResourceManifest _manifest = new();

        static ResourceManagementOptionsConfiguration()
        {
            // This resource will be required for our demo Vue.js application.
            _manifest
                .DefineScript(DemoApp)
                .SetUrl(Root + "/Apps/demo.min.js", Root + "/Apps/demo.js");
        }

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
    }
}

// NEXT STATION: Gulpfile.js
