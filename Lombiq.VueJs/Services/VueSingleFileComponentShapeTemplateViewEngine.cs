using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentShapeTemplateViewEngine : IShapeTemplateViewEngine
{
    public const string CachePrefix = nameof(VueSingleFileComponentShapeTemplateViewEngine) + ":";

    private readonly IShapeTemplateFileProviderAccessor _fileProviderAccessor;
    private readonly IMemoryCache _memoryCache;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;

    public IEnumerable<string> TemplateFileExtensions { get; } = new[] { ".vue" };

    public VueSingleFileComponentShapeTemplateViewEngine(
        IShapeTemplateFileProviderAccessor fileProviderAccessor,
        IMemoryCache memoryCache,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        _fileProviderAccessor = fileProviderAccessor;
        _memoryCache = memoryCache;
        _stringLocalizerFactory = stringLocalizerFactory;
    }

    public async Task<IHtmlContent> RenderAsync(string relativePath, DisplayContext displayContext)
    {
        var cacheName = CachePrefix + relativePath;

        if (_memoryCache.TryGetValue(cacheName, out var cached) && cached is string cachedTemplate)
        {
            return new HtmlString(cachedTemplate);
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

        var localizationRanges = template
            .AllIndexesOf("[[")
            .Where(index => template[(index + 2)..].Contains("]]"))
            .Select(index => new Range(
                index,
                template.IndexOfOrdinal(value: "]]", startIndex: index + 2) + 2))
            .WithoutOverlappingRanges(isSortedByStart: true);

        var stringLocalizer = _stringLocalizerFactory.Create("Vue.js SFC", displayContext.Value.Metadata.Type);

        foreach (var range in localizationRanges.OrderByDescending(range => range.End.Value))
        {
            var (before, expression, after) = template.Partition(range);
            var text = expression[2..^2].Trim();
            template = before + stringLocalizer[text] + after;
        }

        template = FormattableString.Invariant(
            $"<script type=\"x-template\" class=\"{displayContext.Value.Metadata.Type}\">{template}</script>");

        _memoryCache.Set(cacheName, template);
        return new HtmlString(template);
    }

    private static int StartOf(string text, string element) =>
        text.AllIndexesOf("<").First(index => text[(index + 1)..].TrimStart().StartsWithOrdinalIgnoreCase(element));
}
