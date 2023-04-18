using Lombiq.VueJs.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Samples.Controllers;

public class QrCardController : Controller
{
    private readonly IContentManager _contentManager;
    private readonly LinkGenerator _linkGenerator;

    public QrCardController(IContentManager contentManager, LinkGenerator linkGenerator)
    {
        _contentManager = contentManager;
        _linkGenerator = linkGenerator;
    }

    /// <remarks><para>Open this from under /Lombiq.VueJs.Samples/QrCard/Index.</para></remarks>
    public ActionResult Index()
    {
        var apiUrl = _linkGenerator.GetPathByAction(nameof(GetBusinessCard), typeof(QrCardController).ControllerName());

        return View(new QrCardAppViewModel
        {
            ApiUrl = apiUrl,
        });
    }

    public async Task<ActionResult> GetBusinessCard(string cardId) =>
        await _contentManager.GetAsync(cardId) is { } businessCard
            ? Ok(businessCard)
            : NotFound();
}
