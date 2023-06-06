using Lombiq.HelpfulLibraries.Common.Utilities;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentShapeTemplateViewEngine : IShapeTemplateViewEngine
{
    public const string CachePrefix = nameof(VueSingleFileComponentShapeTemplateViewEngine) + ":";

    private readonly IShapeTemplateFileProviderAccessor _fileProviderAccessor;
    private readonly IMemoryCache _memoryCache;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IEnumerable<IVueSingleFileComponentShapeAmender> _amenders;

    public IEnumerable<string> TemplateFileExtensions { get; } = new[] { ".vue" };

    public VueSingleFileComponentShapeTemplateViewEngine(
        IShapeTemplateFileProviderAccessor fileProviderAccessor,
        IMemoryCache memoryCache,
        IStringLocalizerFactory stringLocalizerFactory,
        IEnumerable<IVueSingleFileComponentShapeAmender> amenders)
    {
        _fileProviderAccessor = fileProviderAccessor;
        _memoryCache = memoryCache;
        _stringLocalizerFactory = stringLocalizerFactory;
        _amenders = amenders;
    }

    public async Task<IHtmlContent> RenderAsync(string relativePath, DisplayContext displayContext)
    {
        var template = await GetTemplateAsync(relativePath);

        var localizationRanges = template
            .AllIndexesOf("[[")
            .Where(index => template[(index + 2)..].Contains("]]"))
            .Select(index => new Range(
                index,
                template.IndexOfOrdinal(value: "]]", startIndex: index + 2) + 2))
            .WithoutOverlappingRanges(isSortedByStart: true);

        var shapeName = displayContext.Value.Metadata.Type;
        var stringLocalizer = _stringLocalizerFactory.Create("Vue.js SFC", shapeName);

        foreach (var range in localizationRanges.OrderByDescending(range => range.End.Value))
        {
            var (before, expression, after) = template.Partition(range);
            var text = expression[2..^2].Trim();
            template = before + WebUtility.HtmlEncode(stringLocalizer[text]) + after;
        }

        template = StringHelper.CreateInvariant($"<script type=\"x-template\" class=\"{shapeName}\">{template}</script>");

        var entries = new List<object>();
        foreach (var amender in _amenders) entries.AddRange(await amender.PrependAsync(shapeName));
        entries.Add(new HtmlString(template));
        foreach (var amender in _amenders) entries.AddRange(await amender.AppendAsync(shapeName));

        return new HtmlContentBuilder(entries);
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
