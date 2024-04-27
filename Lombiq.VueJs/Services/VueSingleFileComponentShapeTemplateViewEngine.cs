using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
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

    public IEnumerable<string> TemplateFileExtensions { get; } = new[] { ".vue" };

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

        // Remove all HTML comments. This is done first, because HTML comments take precedence over everything else.
        // This way the contents of comments are guaranteed to not be evaluated.
        template = template
            .GetParenthesisRanges("<!--", "-->")
            .InvertRanges(template.Length)
            .Join(template);

        var shapeName = displayContext.Value.Metadata.Type;
        var builder = new StringBuilder($"<script type=\"x-template\" class=\"{shapeName}\">");

        var localizationRanges = template.GetParenthesisRanges("[[", "]]");
        if (localizationRanges.Count > 0)
        {
            await LocalizeRangesAsync(builder, template, localizationRanges, displayContext);
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
        DisplayContext context)
    {
        var shapeName = context.Value.Metadata.Type;
        var stringLocalizerLazy = new Lazy<IStringLocalizer>(() => _stringLocalizerFactory.Create("Vue.js SFC", shapeName));
        var htmlLocalizerLazy = new Lazy<IHtmlLocalizer>(() => _htmlLocalizerFactory.Create("Vue.js SFC HTML", shapeName));

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

            // Handle HTML localization.
            if (expression[2] == '{' && expression[^3] == '}')
            {
                var value = expression[3..^3].Trim();
                html = htmlLocalizerLazy.Value[value].Html();
            }
            else if (expression[2] == '{')
            {
                var (name, _, input) = expression[3..^2].Partition("}");
                name = name.Trim();
                input = input.Trim();

                if (_converters.FirstOrDefault(converter => converter.IsApplicable(name, input, context)) is not { } converter)
                {
                    throw new InvalidOperationException($"Unknown converter type \"{name}\".");
                }

                html = await converter.ConvertAsync(name, input, context) ?? string.Empty;
            }
            else
            {
                var value = expression[2..^2].Trim();
                html = WebUtility.HtmlEncode(stringLocalizerLazy.Value[value]);
            }

            builder.Append(html);
        }

        // Insert leftover content after the last range.
        builder.Append(template[localizationRanges[^1].End..]);
    }

    private async Task<string> GetTemplateAsync(string relativePath)
    {
        var cacheName = CachePrefix + relativePath;

        if (_memoryCache.TryGetValue(cacheName, out var cached) && cached is string cachedTemplate)
        {
            return cachedTemplate;
        }

        var fileInfo = _fileProviderAccessor.FileProvider.GetFileInfo(relativePath);

        string rawContent;

        await using (var stream = fileInfo.CreateReadStream())
        using (var reader = new StreamReader(stream))
        {
            rawContent = await reader.ReadToEndAsync();
        }

        var templateStarts = StartOf(rawContent, element: "template");
        var scriptStarts = StartOf(rawContent, element: "script");

        var templateOuter = rawContent[templateStarts..scriptStarts];
        var template = rawContent[(templateOuter.IndexOf('>') + 1)..templateOuter.LastIndexOfOrdinal("</")].Trim();

        _memoryCache.Set(cacheName, template);
        return template;
    }

    private static int StartOf(string text, string element) =>
        text.AllIndexesOf("<").First(index => text[(index + 1)..].TrimStart().StartsWithOrdinalIgnoreCase(element));
}
