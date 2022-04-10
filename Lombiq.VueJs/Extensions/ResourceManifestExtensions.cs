using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Constants.ResourceTypes;

namespace Lombiq.VueJs.Extensions;

public static class ResourceManifestExtensions
{
    public static ResourceDefinition DefineSingleFileComponent(this ResourceManifest manifest, string name) =>
        manifest.DefineResource(SingleFileComponent, name);
}
