using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class ContentItemDisplayVueSingleFileComponentShapeAmender(IHttpContextAccessor hca, LinkGenerator linkGenerator)
    : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    private readonly IHttpContextAccessor _hca = hca;
    private readonly LinkGenerator _linkGenerator = linkGenerator;
    protected override string ShapeName => "VueComponent-ContentItemDisplay";
    protected override string PropertyName => "contentItemDisplay";

    protected override ValueTask<object> GetPropertyValueAsync(string shapeName)
    {
        const string dummyContentItemId = "00000000000000000000000000";
        var link = _linkGenerator.GetUriByAction(
            _hca.HttpContext!,
            "Display",
            "Item",
            new { area = "OrchardCore.Contents", contentItemId = dummyContentItemId });

        return new(new
        {
            BaseUrl = link?.Split(dummyContentItemId)[0] ?? "/Contents/ContentItems/",
        });
    }
}
