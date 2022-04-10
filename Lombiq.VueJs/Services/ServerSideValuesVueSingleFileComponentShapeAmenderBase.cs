using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Classes that inherit from this type will prepend some server-side calculated values in a Javascript block before the
/// template. These are accessible through a new property attached to <c>window.Vue</c> (as Vue must already be imported
/// in the header, it is guaranteed to be available here).
/// </summary>
/// <remarks>
/// <para>
/// For example if you set the value of <see cref="PropertyName"/> to <c>myComponent</c> then in Javascript the result
/// of <see cref="GetPropertyValueAsync"/> will be available as <c>window.Vue.$orchardCore.myComponent</c>.
/// </para>
/// </remarks>
public abstract class ServerSideValuesVueSingleFileComponentShapeAmenderBase : IVueSingleFileComponentShapeAmender
{
    /// <summary>
    /// Gets the value used to compare in <see cref="PrependAsync"/>. If this value is <see langword="null"/> then <see
    /// cref="PrependAsync"/> will be called on every Vue.js shape, otherwise only if its name is the value of <see
    /// cref="ShapeName"/>.
    /// </summary>
    protected virtual string ShapeName => null;

    /// <summary>
    /// Gets the name of the property that gets serialized and delivered along with the template.
    /// </summary>
    protected abstract string PropertyName { get; }

    /// <summary>
    /// Turns the value resolved by <see cref="GetPropertyValueAsync"/> into Javascript and appends it before the
    /// template.
    /// </summary>
    public async ValueTask<IEnumerable<IHtmlContent>> PrependAsync(string shapeName)
    {
        if (ShapeName != null && ShapeName != shapeName) return Enumerable.Empty<IHtmlContent>();

        var values = await GetPropertyValueAsync(shapeName);
        var json = JsonConvert.SerializeObject(
            values,
            Formatting.None,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        return new[]
        {
            new HtmlString(
                "<script>" +
                "if (!window.Vue.$orchardCore) window.Vue.$orchardCore = {};" +
                $"window.Vue.$orchardCore[{JsonConvert.SerializeObject(PropertyName)}] = {json};" +
                "</script>"),
        };
    }

    /// <summary>
    /// When awaited, returns the object which will be turned into Javascript value.
    /// </summary>
    protected abstract ValueTask<object> GetPropertyValueAsync(string shapeName);
}
