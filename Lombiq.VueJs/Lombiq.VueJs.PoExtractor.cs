using Lombiq.VueJs.Services;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCoreContrib.PoExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Lombiq.VueJs.Models.TemplateSegment;

public class VueSfcLocalizationProcessor : IProjectProcessor
{
    private readonly ILogger<VueSingleFileComponentProcessor> _logger;
    private readonly IVueSingleFileComponentProcessor _processor;

    public VueSfcLocalizationProcessor()
    {
        _logger = new VueSingleFileComponentProcessorConsoleLogger();
        _processor = new VueSingleFileComponentProcessor([], _logger, new DummyStringLocalizerFactory());
    }

    public void Process(string path, string basePath, LocalizableStringCollection localizableStrings)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentException.ThrowIfNullOrEmpty(basePath);
        ArgumentNullException.ThrowIfNull(localizableStrings);

        var vuePaths = Directory.GetFiles(path, "*.vue", SearchOption.AllDirectories);

        foreach (var vuePath in vuePaths)
        {
            try
            {
                ProcessVueSfcAsync(vuePath, basePath, localizableStrings).Wait();
            }
            catch (AggregateException exceptions)
            {
                foreach (var exception in exceptions.InnerExceptions)
                {
                    _logger.LogError(exception, "Processing Vue SFC file failed (Path: {Path}).", vuePath);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Processing Vue SFC file failed (Path: {Path}).", vuePath);
            }
        }
    }

    private async Task ProcessVueSfcAsync(string path, string basePath, LocalizableStringCollection strings)
    {
        var displayPath = Path.GetRelativePath(Path.GetFullPath(basePath), Path.GetFullPath(path));
        var template = VueSingleFileComponentShapeTemplateViewEngine.ExtractTemplate(await File.ReadAllTextAsync(path));
        var relevantSegments = _processor
            .Process(template)
            .Where(segment => segment.IsLocalizable && segment.ConverterName is StringLocalizerConverterName or HtmlLocalizerConverterName)
            .ToList();
        var converters = _processor.GetConverters(path);

        foreach (var (value, name, _) in relevantSegments)
        {
            var localizer = converters.FirstOrDefault(converter => converter.Name.EqualsOrdinalIgnoreCase(name)) switch
            {
                HtmlLocalizerVueTemplateExpressionConverter htmlConverter =>
                    (DummyStringLocalizerFactory.DummyStringLocalizer)typeof(HtmlLocalizer)
                        .GetField("_localizer", BindingFlags.Instance | BindingFlags.NonPublic)!
                        .GetValue(htmlConverter.HtmlLocalizer)!,
                StringLocalizerVueTemplateExpressionConverter stringConverter =>
                    (DummyStringLocalizerFactory.DummyStringLocalizer)stringConverter.StringLocalizer,
                _ => throw new InvalidOperationException($"Unknown converter type \"{name}\"."),
            };

            _logger.LogInformation(
                "Vue.js SFC string\n\tPath:\t{Path}\n\tType:\t{Type}\n\tValue:\t{Value}\n\tBase:\t{Base}\n",
                displayPath,
                name,
                value,
                localizer.BaseName);

            strings.Add(new()
            {
                Context = localizer.BaseName,
                Location = FindLocation(path, displayPath, value),
                Text = value,
            });
        }
    }

    private LocalizableStringLocation FindLocation(string path, string displayPath, string value)
    {
        var location = new LocalizableStringLocation { SourceFile = displayPath };
        var file = File.ReadAllText(path);
        var index = file.IndexOf(value, StringComparison.Ordinal);

        if (index < 0) return location;

        var precedingLines = file[..index].Split('\n');
        location.SourceFileLine = precedingLines.Length;
        location.Comment = precedingLines[^1].TrimStart() + value.RegexReplace(@"\s*\n+\s*", "¶"); // Multiline comments are not supported.
        return location;
    }
}

public class VueSingleFileComponentProcessorConsoleLogger : ILogger<VueSingleFileComponentProcessor>
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
        Console.WriteLine(
            "[{0}] {1}: {2}{3}",
            logLevel,
            eventId,
            formatter(state, exception),
            exception is null ? string.Empty : $"\n\t{exception}");
}

public class DummyStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) =>
        Create(resourceSource.Name, resourceSource.Namespace);

    public IStringLocalizer Create(string baseName, string location) =>
        new DummyStringLocalizer(baseName, location);

    public class DummyStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<(string, object[]), LocalizedString> _strings = new();

        public string BaseName { get; }
        public string Location { get; }

        public DummyStringLocalizer(string baseName, string location)
        {
            BaseName = baseName;
            Location = location;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            _strings.Values;

        public LocalizedString this[string name] => this[name, []];

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (!_strings.TryGetValue((name, arguments), out var value))
                {
                    value = new LocalizedString(name, string.Format(name, arguments));
                    _strings[(name, arguments)] = value;
                }

                return value;
            }
        }
    }
}

// ProjectProcessors.Add(new VueSfcLocalizationProcessor());
