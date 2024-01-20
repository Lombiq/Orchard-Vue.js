using OrchardCore.Localization;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class DateTimeVueSingleFileComponentShapeAmender(
    ILocalClock localClock,
    ILocalizationService localizationService) : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    protected override string ShapeName => "VueComponent-DateTime";
    protected override string PropertyName => "dateTime";

    protected override async ValueTask<object> GetPropertyValueAsync(string shapeName) =>
        new
        {
            TimeZone = (await localClock.GetLocalTimeZoneAsync()).TimeZoneId,
            Culture = await localizationService.GetDefaultCultureAsync(),
        };
}
