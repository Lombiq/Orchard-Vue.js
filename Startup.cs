using Lombiq.VueJs.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.VueJs
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IShapeTemplateHarvester, VueComponentTemplateHarvester>();
            services.AddScoped<IResourceManifestProvider, ResourceManifest>();
        }
    }
}