using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.VueJs.Resources.Constants;

namespace Lombiq.VueJs.Resources;

public class ResourceFilterProvider : IResourceFilterProvider
{
    public void AddResourceFilter(ResourceFilterBuilder builder) =>
        builder.Always().RegisterHeadScript(ResourceNames.SetupEnvironment);
}
