using Lombiq.VueJs.Services;
using Lombiq.VueJs.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.Modules;

namespace Lombiq.VueJs;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IShapeTemplateHarvester, VueComponentTemplateHarvester>();

        services.AddScoped<IShapeTemplateHarvester, VueSingleFileComponentTemplateHarvester>();
        services.AddScoped<IShapeTemplateViewEngine, VueSingleFileComponentShapeTemplateViewEngine>();
        services.AddTagHelpers<VueComponentTagHelper>();

        services.AddScoped<IVueSingleFileComponentShapeAmender, ContentItemDisplayVueSingleFileComponentShapeAmender>();
        services.AddScoped<IVueSingleFileComponentShapeAmender, DateTimeVueSingleFileComponentShapeAmender>();
    }
}
