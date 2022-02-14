using Lombiq.VueJs.Constants;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.VueJs
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static readonly ResourceManifest _manifest = new();

        static ResourceManagementOptionsConfiguration() =>
            // ES6 Promise polyfill for IE. Use it as a dependency for Vue apps that uses Promises (e.g. axios).
            _manifest
                .DefineScript(ResourceNames.Es6PromisePolyfill)
                .SetUrl("/Lombiq.VueJs/es6-promise.auto.min.js", "/Lombiq.VueJs/es6-promise.auto.js");

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
    }
}
