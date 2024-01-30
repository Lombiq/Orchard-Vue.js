using OrchardCore.Localization;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class DateTimeVueSingleFileComponentShapeAmender : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    private readonly ILocalClock _localClock;
    private readonly ILocalizationService _localizationService;

    protected override string ShapeName => "VueComponent-DateTime";
    protected override string PropertyName => "dateTime";

    public DateTimeVueSingleFileComponentShapeAmender(
        ILocalClock localClock,
        ILocalizationService localizationService)
    {
        _localClock = localClock;
        _localizationService = localizationService;
    }

    protected override async ValueTask<object> GetPropertyValueAsync(string shapeName) =>
        new
        {
            TimeZone = (await _localClock.GetLocalTimeZoneAsync()).TimeZoneId,
            Culture = await _localizationService.GetDefaultCultureAsync(),
        };
}
