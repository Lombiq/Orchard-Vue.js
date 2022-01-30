using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services
{
    public class VueSingleFileComponentShapeTemplateViewEngine : IShapeTemplateViewEngine
    {
        public const string CachePrefix = nameof(VueSingleFileComponentShapeTemplateViewEngine) + ":";

        private readonly IMemoryCache _memoryCache;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public IEnumerable<string> TemplateFileExtensions { get; } = new[] { ".vue" };

        public VueSingleFileComponentShapeTemplateViewEngine(
            IMemoryCache memoryCache,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _memoryCache = memoryCache;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public async Task<IHtmlContent> RenderAsync(string relativePath, DisplayContext displayContext)
        {
            if (_memoryCache.TryGetValue(CachePrefix + relativePath, out var cached) && cached is string template)
            {
                return new HtmlString(template);
            }

            var rawContent = await File.ReadAllTextAsync(relativePath);

            // A Vue SFC is neither real XML nor real HTML so for once RegEx is actually safer within the
            // constraints of the known SFC outline. Might make a custom parser for it later.
            var afterStart = rawContent.RegexReplace(
                @"^.*<\s*template[^>]*>",
                string.Empty,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            template = afterStart.RegexReplace(
                @"<\s*/\s*template[^>]*>.*$",
                string.Empty,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var shapeName = displayContext.Value.Id;
            var stringLocalizer = _stringLocalizerFactory.Create("Vue.js SFC", shapeName);

            var localizationRanges = template
                .AllIndexesOf("[[")
                .Select(index => new Range(
                    index,
                    template.IndexOf(
                        value: "]]",
                        startIndex: index + 2,
                        StringComparison.Ordinal)))
                .WithoutOverlappingRanges(isSortedByStart: true);

            foreach (var range in localizationRanges)
            {
                var (before, expression, after) = template.Partition(range);
                var text = expression[2..^2].Trim();
                template = before + stringLocalizer[text] + after;
            }

            _memoryCache.Set(CachePrefix + relativePath, template);

            return new HtmlString(FormattableString.Invariant(
                $"<script type=\"x-template\" class=\"{shapeName}\">{shapeName}</script>"));
        }
    }
}
