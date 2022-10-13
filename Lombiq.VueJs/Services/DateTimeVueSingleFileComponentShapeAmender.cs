using OrchardCore;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class DateTimeVueSingleFileComponentShapeAmender : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    private readonly ILocalClock _localClock;
    private readonly IOrchardHelper _orchardHelper;

    protected override string ShapeName => "VueComponent-DateTime";
    protected override string PropertyName => "dateTime";

    public DateTimeVueSingleFileComponentShapeAmender(ILocalClock localClock, IOrchardHelper orchardHelper)
    {
        _localClock = localClock;
        _orchardHelper = orchardHelper;
    }

    protected override async ValueTask<object> GetPropertyValueAsync(string shapeName) =>
        new
        {
            TimeZone = (await _localClock.GetLocalTimeZoneAsync()).TimeZoneId,
            Culture = _orchardHelper.CultureName(), // Same as "@Orchard.CultureName()".
        };
}
