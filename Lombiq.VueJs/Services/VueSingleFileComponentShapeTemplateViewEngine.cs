using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Modules.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentShapeTemplateViewEngine : IShapeTemplateViewEngine
{
    public const string CachePrefix = nameof(VueSingleFileComponentShapeTemplateViewEngine) + ":";

    private readonly IShapeTemplateFileProviderAccessor _fileProviderAccessor;
    private readonly IMemoryCache _memoryCache;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IHtmlLocalizerFactory _htmlLocalizerFactory;
    private readonly ILogger<VueSingleFileComponentShapeTemplateViewEngine> _logger;
    private readonly IEnumerable<IVueSingleFileComponentShapeAmender> _amenders;
    private readonly IEnumerable<IVueTemplateExpressionConverter> _converters;

    public IEnumerable<string> TemplateFileExtensions { get; } = [".vue"];

    public VueSingleFileComponentShapeTemplateViewEngine(
        IShapeTemplateFileProviderAccessor fileProviderAccessor,
        IMemoryCache memoryCache,
        IStringLocalizerFactory stringLocalizerFactory,
        IHtmlLocalizerFactory htmlLocalizerFactory,
        ILogger<VueSingleFileComponentShapeTemplateViewEngine> logger,
        IEnumerable<IVueSingleFileComponentShapeAmender> amenders,
        IEnumerable<IVueTemplateExpressionConverter> converters)
    {
        _fileProviderAccessor = fileProviderAccessor;
        _memoryCache = memoryCache;
        _stringLocalizerFactory = stringLocalizerFactory;
        _htmlLocalizerFactory = htmlLocalizerFactory;
        _logger = logger;
        _amenders = amenders;
        _converters = converters;
    }

    public async Task<IHtmlContent> RenderAsync(string relativePath, DisplayContext displayContext)
    {
        var template = await GetTemplateAsync(relativePath);

        var shapeName = displayContext.Value.Metadata.Type;
        var builder = new StringBuilder($"<script type=\"x-template\" class=\"{shapeName}\">");

        var localizationRanges = template.GetParenthesisRanges("[[", "]]");
        if (localizationRanges.Count > 0)
        {
            var fileName = Path.GetFileName(relativePath);

            var stringLocalizerLazy = new Lazy<IStringLocalizer>(() => _stringLocalizerFactory.Create(fileName, relativePath));
            var htmlLocalizerLazy = new Lazy<IHtmlLocalizer>(() => _htmlLocalizerFactory.Create(fileName + ".html", relativePath));

            await LocalizeRangesAsync(
                builder,
                template,
                localizationRanges,
                displayContext,
                stringLocalizerLazy,
                htmlLocalizerLazy);
        }
        else
        {
            builder.Append(template);
        }

        builder.Append("</script>");

        var entries = new List<object>();
        foreach (var amender in _amenders) entries.AddRange(await amender.PrependAsync(shapeName));
        entries.Add(new HtmlString(builder.ToString()));
        foreach (var amender in _amenders) entries.AddRange(await amender.AppendAsync(shapeName));

        return new HtmlContentBuilder(entries);
    }

    private async Task LocalizeRangesAsync(
        StringBuilder builder,
        string template,
        IList<Range> localizationRanges,
        DisplayContext context,
        Lazy<IStringLocalizer> stringLocalizerLazy,
        Lazy<IHtmlLocalizer> htmlLocalizerLazy)
    {
        var startIndex = new Index(0);
        foreach (var range in localizationRanges)
        {
            // Insert content before this range.
            builder.Append(template[startIndex..range.Start]);
            startIndex = range.End;

            var expression = template[range];
            string html;

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
                if (_converters.FirstOrDefault(converter => converter.IsApplicable(name, value, context)) is not { } converter)
                {
                    throw new InvalidOperationException($"Unknown converter type \"{name}\".");
                }

                html = await converter.ConvertAsync(name, value, context) ?? string.Empty;
            }
            else
            {
                html = ConvertLocalization(expression, stringLocalizerLazy, htmlLocalizerLazy);
            }

            builder.Append(html);
        }

        // Insert leftover content after the last range.
        builder.Append(template[localizationRanges[^1].End..]);
    }

    private async Task<string> GetTemplateAsync(string relativePath)
    {
        var cacheName = CachePrefix + relativePath;

        if (!_memoryCache.TryGetValue(cacheName, out var cached) || cached is not string { Length: > 0 } cachedTemplate)
        {
            var fileInfo = _fileProviderAccessor.FileProvider.GetFileInfo(relativePath);
            var rawContent = string.Join('\n', await fileInfo.ReadAllLinesAsync());

            return _memoryCache.Set(cacheName, ExtractTemplate(rawContent));
        }

        return cachedTemplate;
    }

    /// <summary>
    /// Gets the top template element from the <c>.vue</c> file contents.
    /// </summary>
    public static string ExtractTemplate(string rawContent)
    {
        // Remove all HTML comments. This is done first, because HTML comments take precedence over everything else.
        // This way the contents of comments are guaranteed to not be evaluated.
        rawContent = rawContent
            .GetParenthesisRanges("<!--", "-->")
            .InvertRanges(rawContent.Length)
            .Join(rawContent);

        var templateStarts = StartOf(rawContent, element: "template");
        var scriptStarts = StartOf(rawContent, element: "script");
        var templateOuter = rawContent[templateStarts..scriptStarts];

        return rawContent[(templateOuter.IndexOf('>') + 1)..templateOuter.LastIndexOfOrdinal("</")].Trim();
    }

    /// <summary>
    /// Use <see cref="IHtmlLocalizer"/> if the <paramref name="expression"/> fits the <c>[[{ ... }]]</c> pattern, or
    /// use <see cref="IStringLocalizer"/> if it fits the <c>[[ ... ]]</c> pattern, to localize the text content inside
    /// the brackets.
    /// </summary>
    public static string ConvertLocalization(
        string expression,
        Lazy<IStringLocalizer> stringLocalizerLazy,
        Lazy<IHtmlLocalizer> htmlLocalizerLazy)
    {
        if (expression[2] == '{' && expression[^3] == '}')
        {
            var value = expression[3..^3].Trim();
            return htmlLocalizerLazy.Value[value].Html();
        }

        return WebUtility.HtmlEncode(
            stringLocalizerLazy.Value[expression[2..^2].Trim()]);
    }

    public static bool IsNamedConverterExpression(string expression, out string name, out string value)
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

    private static int StartOf(string text, string element) =>
        text.AllIndexesOf("<").First(index => text[(index + 1)..].TrimStart().StartsWithOrdinalIgnoreCase(element));
}
