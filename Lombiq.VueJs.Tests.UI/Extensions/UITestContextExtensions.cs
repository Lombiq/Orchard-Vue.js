using Lombiq.HelpfulLibraries.Libraries.Mvc;
using Lombiq.Tests.UI.Services;
using Lombiq.VueJs.Samples.Controllers;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Tests.UI.Extensions;

public static class UITestContextExtensions
{
    public static Task GoToVueAppAsync(this UITestContext context) =>
        context.GoToAsync<VueAppController>(controller => controller.Index());

    public static Task GoToVueSfcAsync(this UITestContext context) =>
        context.GoToAsync<VueSfcController>(controller => controller.Index());

    public static Task GoToVueSfcAppTagHelperAsync(this UITestContext context) =>
        context.GoToAsync<VueSfcController>(controller => controller.AppTagHelper());

    public static Task GoToVueSfcEnhancedListAsync(this UITestContext context, int page = 1) =>
        context.GoToAsync<VueSfcController>(controller => controller.EnhancedList(page));
}
