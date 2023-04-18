using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Lombiq.VueJs.Samples.Migrations;
using Lombiq.VueJs.Samples.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.VueJs.Samples;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services) =>
        services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>()
            .AddScoped<IDataMigration, BusinessCardMigrations>()
            .AddTagHelpers<QrCodeTagHelper>()
            .AddLazyInjectionSupport();
}
