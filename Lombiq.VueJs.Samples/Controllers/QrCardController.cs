using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Samples.Controllers;

[ApiController]
[Route("business-card")]
[IgnoreAntiforgeryToken]
public class QrCardController : Controller
{
    private readonly IContentManager _contentManager;

    public QrCardController(IContentManager contentManager) =>
        _contentManager = contentManager;

    public ActionResult Index() => View();

    [HttpGet("{cardId}")]
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
