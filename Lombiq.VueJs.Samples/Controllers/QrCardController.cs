using Lombiq.VueJs.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.VueJs.Samples.Controllers;

public class QrCardController(IContentManager contentManager, Lazy<LinkGenerator> linkGenerator) : Controller
{
    /// <remarks><para>Open this from under /Lombiq.VueJs.Samples/QrCard/Index.</para></remarks>
    [ContentSecurityPolicy(Blob, WorkerSrc, ScriptSrc)]
    public ActionResult Index()
    {
        var apiUrl = linkGenerator.Value.GetPathByAction(nameof(GetBusinessCard), typeof(QrCardController).ControllerName());

        return View(new QrCardAppViewModel
        {
            ApiUrl = apiUrl,
        });
    }

    public async Task<ActionResult> GetBusinessCard(string cardId) =>
        await contentManager.GetAsync(cardId) is { } businessCard
            ? Ok(businessCard)
            : NotFound();
}
