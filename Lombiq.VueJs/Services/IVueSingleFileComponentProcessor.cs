using Lombiq.VueJs.Models;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Service that provides functionality related to processing Vue.js single file components.
/// </summary>
public interface IVueSingleFileComponentProcessor
{
    /// <summary>
    /// If there are any localizable segments in the provided <paramref name="template"/> (the HTML content inside the
    /// SFC's <c>$lt;template&gt;</c> element), returns a sequence of strings that indicate for each segment if they are
    /// localizable and with what converter. If there are no localizable segments, it returns the <paramref
    /// name="template"/> in a single segment.
    /// </summary>
    IEnumerable<TemplateSegment> Process(string template);

    /// <summary>
    /// Returns a collection of expression converters, including the <see cref="IStringLocalizer{TResource}"/> and <see
    /// cref="IHtmlLocalizer{TResource}"/> that use <paramref name="relativePath"/> as their context.
    /// </summary>
    ICollection<IVueTemplateExpressionConverter> GetConverters(string relativePath);
}
