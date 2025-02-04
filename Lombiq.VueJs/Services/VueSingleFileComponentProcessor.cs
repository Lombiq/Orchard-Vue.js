using Lombiq.VueJs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentProcessor : IVueSingleFileComponentProcessor
{
    private readonly ILogger<VueSingleFileComponentProcessor> _logger;

    public VueSingleFileComponentProcessor(ILogger<VueSingleFileComponentProcessor> logger) => _logger = logger;

    public IEnumerable<TemplateSegment> Process(string template)
    {
        var localizationRanges = template.GetParenthesisRanges("[[", "]]");
        if (localizationRanges.Count == 0)
        {
            yield return TemplateSegment.NonLocalizable(template);
            yield break;
        }

        var startIndex = new Index(0);
        foreach (var range in localizationRanges)
        {
            // Insert content before this range.
            yield return TemplateSegment.NonLocalizable(template[startIndex..range.Start]);
            startIndex = range.End;

            var expression = template[range];

            // Include a logger warning if the inner spacing is missing. This will cause failures e.g. during UI tests,
            // and so ensure correct formatting.
            if (expression[2] is not '{' and not ' ')
            {
                _logger.LogWarning(
                    "Vue SFC localization strings should follow the following formats: [[ text ]], [[{{ html }}]] or " +
                    "[[{{converter}} input ]]. Please include the inner spacing to ensure future compatibility. Your " +
                    "expression was: \"{Expression}\".",
                    expression);
            }

            if (IsNamedConverterExpression(expression, out var name, out var value))
            {
                yield return new(value, name);
            }
            else if (expression[2] == '{' && expression[^3] == '}')
            {
                yield return TemplateSegment.HtmlLocalizer(expression[3..^3].Trim());
            }
            else
            {
                yield return TemplateSegment.StringLocalizer(expression[2..^2].Trim());
            }
        }

        // Insert leftover content after the last range.
        yield return TemplateSegment.NonLocalizable(template[localizationRanges[^1].End..]);
    }

    private static bool IsNamedConverterExpression(string expression, out string name, out string value)
    {
        if (expression[2] != '{' || expression[^3] == '}')
        {
            name = null;
            value = null;
            return false;
        }

        (name, _, value) = expression[3..^2].Partition("}");
        name = name.Trim();
        value = value.Trim();
        return true;
    }
}
