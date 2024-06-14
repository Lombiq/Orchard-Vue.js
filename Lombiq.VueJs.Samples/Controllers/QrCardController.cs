using Lombiq.VueJs.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.VueJs.Samples.Controllers;

public class QrCardController : Controller
{
    private readonly IContentManager _contentManager;

    public QrCardController(IContentManager contentManager) =>
        _contentManager = contentManager;

    /// <remarks><para>Open this from under /Lombiq.VueJs.Samples/QrCard/Index.</para></remarks>
    [ContentSecurityPolicy(Blob, WorkerSrc, ScriptSrc)]
    public ActionResult Index() =>
        View(new QrCardAppViewModel
        {
            ApiUrl = Url.Action(nameof(GetBusinessCard)),
        });

    public async Task<ActionResult> GetBusinessCard(string cardId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return await _contentManager.GetAsync(cardId) is { } businessCard
            ? Ok(businessCard)
            : NotFound();
    }
}
