using Lombiq.VueJs.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Samples.Controllers;

public class QrCardController : Controller
{
    private readonly IContentManager _contentManager;

    public QrCardController(IContentManager contentManager) =>
        _contentManager = contentManager;

    // Open this from under /Lombiq.VueJs.Samples/QrCard/Index.
    public ActionResult Index()
    {
        var linkGenerator = HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

        var apiUrl = linkGenerator.GetPathByAction(nameof(GetBusinessCard), typeof(QrCardController).ControllerName());

        return View(new QrCardAppViewModel
        {
            ApiUrl = apiUrl,
        });
    }

    public async Task<ActionResult> GetBusinessCard(string cardId)
    {
        var businessCard = await _contentManager.GetAsync(cardId);
        if (businessCard is null)
        {
            return NotFound();
        }

        return Ok(businessCard);
    }
}
