using Microsoft.AspNetCore.Mvc;

namespace Lombiq.VueJs.Samples.Controllers
{
    // This controller is for returning a Vue.js Single File Component (abbreviated as SFC going forward) in an MVC
    // view. However, you could use any other ways of doing it such as injecting as a shape or using widgets.
    public class VueSfcController : Controller
    {
        [HttpGet]
        // Open this from under /Lombiq.VueJs.Samples/VueSfc/Index
        public ActionResult Index() => View();

        // NEXT STATION: Views/VueSfc/Index.cshtml
    }
}
