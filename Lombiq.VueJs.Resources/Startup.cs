using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;

namespace Lombiq.VueJs.Resources;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddResourceManagementConfiguration<ResourceManagementOptionsConfiguration>();
        services.AddResourceFilter<ResourceFilterProvider>();
    }

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        app.UseResourceFilters();
}
