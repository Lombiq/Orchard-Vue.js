using OrchardCore.Modules;
using System.Globalization;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class DateTimeVueSingleFileComponentShapeAmender : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    private readonly ILocalClock _localClock;

    protected override string ShapeName => "VueComponent-DateTime";
    protected override string PropertyName => "dateTime";

    public DateTimeVueSingleFileComponentShapeAmender(ILocalClock localClock) => _localClock = localClock;

    protected override async ValueTask<object> GetPropertyValueAsync(string shapeName) =>
        new
        {
            TimeZone = (await _localClock.GetLocalTimeZoneAsync()).TimeZoneId,
            Culture = CultureInfo.CurrentUICulture.Name, // Same as "@Orchard.CultureName()".
        };
}
