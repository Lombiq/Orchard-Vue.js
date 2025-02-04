using Lombiq.VueJs.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCoreContrib.PoExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.VueJs.Models.TemplateSegment;

public class VueSfcLocalizationProcessor : IProjectProcessor
{
    private readonly IVueSingleFileComponentProcessor _processor;

    public VueSfcLocalizationProcessor() =>
        _processor = new VueSingleFileComponentProcessor(
            [],
            new VueSingleFileComponentProcessorConsoleLogger(),
            new DummyStringLocalizerFactory());

    public void Process(string path, string basePath, LocalizableStringCollection strings)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentException.ThrowIfNullOrEmpty(basePath);
        ArgumentNullException.ThrowIfNull(strings);

        var vuePaths = Directory.GetFiles(path, "*.vue", SearchOption.AllDirectories);

        foreach (var vuePath in vuePaths)
        {
            try
            {
                ProcessVueSfcAsync(vuePath, basePath, strings).Wait();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Processing Vue SFC file failed for: {0}\n{1}", vuePath, exception);
            }
        }
    }

    private async Task ProcessVueSfcAsync(string path, string basePath, LocalizableStringCollection strings)
    {
        var template = VueSingleFileComponentShapeTemplateViewEngine.ExtractTemplate(await File.ReadAllTextAsync(path));
        var relevantSegments = _processor
            .Process(template)
            .Where(segment => segment.IsLocalizable && segment.ConverterName is StringLocalizerConverterName or HtmlLocalizerConverterName)
            .ToList();

        foreach (var (value, name, _) in relevantSegments)
        {
            Console.WriteLine("{0}\n\tType:\t{1}\n\tValue:\t{2}\n\n\n\n", path, name, value);
        }

        // if (json is JsonObject jsonObject)
        // {
        //     foreach (var (name, value) in jsonObject)
        //     {
        //         var newPrefix = string.IsNullOrEmpty(prefix) ? name : $"{prefix}.{name}";
        //         ProcessVueSfc(path, strings, value, newPrefix);
        //     }
        //
        //     return;
        // }
        //
        // if (json is JsonValue jsonValue)
        // {
        //     var value = jsonValue.GetObjectValue()?.ToString();
        //     strings.Add(new()
        //     {
        //         Context = prefix,
        //         Location = new() { SourceFile = path },
        //         Text = value,
        //     });
        // }
    }
}

public class VueSingleFileComponentProcessorConsoleLogger : ILogger<VueSingleFileComponentProcessor>
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
        Console.WriteLine("[{0}] {1}: {2}", logLevel, eventId, formatter(state, exception));
}

public class DummyStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) =>
        Create(resourceSource.Name, resourceSource.Namespace);

    public IStringLocalizer Create(string baseName, string location) =>
        new DummyStringLocalizer(baseName, location);

    private class DummyStringLocalizer : IStringLocalizer
    {
        private readonly string _baseName;
        private readonly string _location;
        private readonly Dictionary<(string, object[]), LocalizedString> _strings = new();

        public DummyStringLocalizer(string baseName, string location)
        {
            _baseName = baseName;
            _location = location;
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
