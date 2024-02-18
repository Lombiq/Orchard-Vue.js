using Lombiq.VueJs.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.VueJs.Samples.Constants.FeatureIds;
using static Lombiq.VueJs.Samples.Constants.ResourceNames;

namespace Lombiq.VueJs.Samples;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        // This resource is not strictly required, but it tells the <vue-component> tag helper which other shapes it
        // needs to import. As you can see we don't use SetUrl. Only SetDependencies is used if there are any.
        _manifest
            .DefineSingleFileComponent(DemoSfc)
            // This is resolved recursively so you don't need to add more than the direct child components.
            .SetDependencies(DemoRepeater);

        // We don't need to define an SFC resource for DemoRepeater, because it doesn't have child components. If you
        // define an empty resource definition such as "_manifest.DefineSingleFileComponent(DemoRepeater);" it will be
        // ignored.

        // On the other hand make sure your .vue files are embedded during build, e.g. include this in the csproj:
        // <ItemGroup><EmbeddedResource Include="Assets\Scripts\VueComponents\*.vue" /></ItemGroup>

        _manifest
            .DefineSingleFileComponent(BusinessCard)
            .SetDependencies(LoadingIndicator);

        _manifest
            .DefineSingleFileComponent(QrCard)
            .SetDependencies(LoadingIndicator, BusinessCard);
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}

// NEXT STATION: package.json
