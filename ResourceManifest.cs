using Lombiq.VueJs.Constants;
using Orchard.UI.Resources;

namespace Lombiq.VueJs
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // ES6 Promise polyfill for IE. Use it as a dependency for Vue apps that uses Promises (e.g. axios).
            manifest
                .DefineScript(ResourceNames.Es6PromisePolyfill)
                .SetUrl("/Lombiq.VueJs/es6-promise.auto.min.js", "/Lombiq.VueJs/es6-promise.auto.js");
        }
    }
}