using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Modules.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentShapeTemplateViewEngine : IShapeTemplateViewEngine
{
    public const string CachePrefix = nameof(VueSingleFileComponentShapeTemplateViewEngine) + ":";

    private readonly IShapeTemplateFileProviderAccessor _fileProviderAccessor;
    private readonly IMemoryCache _memoryCache;
    private readonly IEnumerable<IVueSingleFileComponentShapeAmender> _amenders;
    private readonly IVueSingleFileComponentProcessor _processor;

    public IEnumerable<string> TemplateFileExtensions { get; } = [".vue"];

    public VueSingleFileComponentShapeTemplateViewEngine(
        IShapeTemplateFileProviderAccessor fileProviderAccessor,
        IMemoryCache memoryCache,
        IEnumerable<IVueSingleFileComponentShapeAmender> amenders,
        IVueSingleFileComponentProcessor processor)
    {
        _fileProviderAccessor = fileProviderAccessor;
        _memoryCache = memoryCache;
        _amenders = amenders;
        _processor = processor;
    }

    public async Task<IHtmlContent> RenderAsync(string relativePath, DisplayContext displayContext)
    {
        var template = await GetTemplateAsync(relativePath);

        var shapeName = displayContext.Value.Metadata.Type;
        var builder = new StringBuilder($"<script type=\"x-template\" class=\"{shapeName}\">");

        var converters = _processor.GetConverters(relativePath);
        foreach (var (value, name, isLocalizable) in _processor.Process(template))
        {
            var html = value;

            if (isLocalizable)
            {
                if (converters.FirstOrDefault(converter => converter.IsApplicable(name, value, displayContext)) is not { } converter)
                {
                    throw new InvalidOperationException($"Unknown converter type \"{name}\".");
                }

                html = await converter.ConvertAsync(name, value, displayContext) ?? string.Empty;
            }

            builder.Append(html);
        }

        builder.Append("</script>");

        var entries = new List<object>();
        foreach (var amender in _amenders) entries.AddRange(await amender.PrependAsync(shapeName));
        entries.Add(new HtmlString(builder.ToString()));
        foreach (var amender in _amenders) entries.AddRange(await amender.AppendAsync(shapeName));

        return new HtmlContentBuilder(entries);
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

    private static int StartOf(string text, string element) =>
        text.AllIndexesOf("<").First(index => text[(index + 1)..].TrimStart().StartsWithOrdinalIgnoreCase(element));
}
