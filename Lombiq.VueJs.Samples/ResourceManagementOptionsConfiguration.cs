using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.TrainingDemo
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static readonly ResourceManifest _manifest = new();

        static ResourceManagementOptionsConfiguration()
        {
            // This resource will be required for our demo Vue.js application.
            _manifest
                .DefineScript("Lombiq.TrainingDemo.DemoApp")
                .SetUrl("~/Lombiq.TrainingDemo/Apps/demo.min.js", "~/Lombiq.TrainingDemo/Apps/demo.js");
        }

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
    }
}

// NEXT STATION: Gulpfile.js
