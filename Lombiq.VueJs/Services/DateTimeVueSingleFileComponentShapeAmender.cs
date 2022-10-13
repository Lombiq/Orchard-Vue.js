using OrchardCore.DisplayManagement.Razor;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class DateTimeVueSingleFileComponentShapeAmender : ServerSideValuesVueSingleFileComponentShapeAmenderBase
{
    private readonly ILocalClock _localClock;
    private readonly IOrchardDisplayHelper _displayHelper;

    protected override string ShapeName => "VueComponent-DateTime";
    protected override string PropertyName => "dateTime";

    public DateTimeVueSingleFileComponentShapeAmender(ILocalClock localClock, IOrchardDisplayHelper displayHelper)
    {
        _localClock = localClock;
        _displayHelper = displayHelper;
    }

    protected override async ValueTask<object> GetPropertyValueAsync(string shapeName) =>
        new
        {
            TimeZone = (await _localClock.GetLocalTimeZoneAsync()).TimeZoneId,
            Culture = _displayHelper.CultureName(), // Same as "@Orchard.CultureName()".
        };
}
