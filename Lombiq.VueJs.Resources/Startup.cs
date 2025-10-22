using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace Lombiq.VueJs.Resources;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services) =>
        services.AddResourceManagementConfiguration<ResourceManagementOptionsConfiguration>();
}
