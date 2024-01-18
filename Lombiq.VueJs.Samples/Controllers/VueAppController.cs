using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lombiq.VueJs.Samples.Controllers;

// This controller is for returning a Vue.js application in an MVC view. However, you could use any other ways of doing
// it such as injecting as a shape or using widgets.
public class VueAppController : Controller
{
    [HttpGet]
    [ScriptUnsafeEval]
    // Open this from under /Lombiq.VueJs.Samples/VueApp/Index
    public ActionResult Index() => View();

    // NEXT STATION: Views/VueApp/Index.cshtml
}
