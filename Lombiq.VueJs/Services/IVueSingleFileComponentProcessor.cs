using Lombiq.VueJs.Models;
using System.Collections.Generic;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Splits a provided Vue SFC template string into segments that indicate if they can be localized and if yes, what <see
/// cref="IVueTemplateExpressionConverter"/> should be used.
/// </summary>
public interface IVueSingleFileComponentProcessor
{
    /// <summary>
    /// If there are any localizable segments in the provided <paramref name="template"/>, returns a sequence of
    /// non-localizable and localizable strings. Otherwise, returns <paramref name="template"/> in a single segment.
    /// </summary>
    IEnumerable<TemplateSegment> Process(string template);
}
