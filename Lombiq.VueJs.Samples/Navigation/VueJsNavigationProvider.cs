using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Lombiq.VueJs.Samples.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Lombiq.VueJs.Samples.Navigation;

public class VueJsNavigationProvider : MainMenuNavigationProviderBase
{
    public VueJsNavigationProvider(
        IHttpContextAccessor hca,
        IStringLocalizer<VueJsNavigationProvider> stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }

    protected override void Build(NavigationBuilder builder) =>
        builder
            .Add(T["Vue.js"], builder => builder
                .Add(T["Vue Javascript App"], itemBuilder => itemBuilder
                    .Action<VueAppController>(_hca.HttpContext, controller => controller.Index()))
                .Add(T["Vue Single File Component (SFC)"], itemBuilder => itemBuilder
                    .Action<VueSfcController>(_hca.HttpContext, controller => controller.Index()))
                .Add(T["Standalone SFC with Tag Helper"], itemBuilder => itemBuilder
                    .Action<VueSfcController>(_hca.HttpContext, controller => controller.AppTagHelper()))
                .Add(T["Progressive Enhancement"], itemBuilder => itemBuilder
                    .Action<VueSfcController>(_hca.HttpContext, controller => controller.EnhancedList(1)))
                .Add(T["QR Card"], itemBuilder => itemBuilder
                    .Action<QrCardController>(_hca.HttpContext, controller => controller.Index())));
}
