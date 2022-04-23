using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Lets you prepend or append HTML content to the Vue.js template outputted by the shape.
/// </summary>
public interface IVueSingleFileComponentShapeAmender
{
    /// <summary>
    /// Lets you prepend content to the Vue.js template outputted by the shape. The <paramref name="shapeName"/> is
    /// provided to help limit the scope, but you can also ignore it to amend every Vue.js shape.
    /// </summary>
    ValueTask<IEnumerable<IHtmlContent>> PrependAsync(string shapeName) => new(Enumerable.Empty<IHtmlContent>());

    /// <summary>
    /// Lets you append content to the Vue.js template outputted by the shape. The <paramref name="shapeName"/> is
    /// provided to help limit the scope, but you can also ignore it to amend every Vue.js shape.
    /// </summary>
    ValueTask<IEnumerable<IHtmlContent>> AppendAsync(string shapeName) => new(Enumerable.Empty<IHtmlContent>());
}
