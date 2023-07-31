using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.Services;
using Lombiq.VueJs.Tests.UI.Assets.Media;
using System;

namespace Lombiq.VueJs.Tests.UI.Extensions;

public static class BrowserConfigurationExtensions
{
    public static void ConfigureFakeVideoSourceForNegativeTest(this BrowserConfiguration browserConfiguration) =>
        browserConfiguration.ConfigureFakeVideoSource("richard.roe.mjpeg");

    public static void ConfigureFakeVideoSourceForPositiveTest(this BrowserConfiguration browserConfiguration) =>
        browserConfiguration.ConfigureFakeVideoSource("john.doe.mjpeg");

    private static void ConfigureFakeVideoSource(this BrowserConfiguration browserConfiguration, string resource) =>
        browserConfiguration.FakeVideoSource = new FakeBrowserVideoSource
        {
            StreamProvider = () =>
            {
                // Get the resource stream from the assembly.
                var stream = typeof(DirectoryPlaceholder)
                    .Assembly
                    .GetManifestResourceStream(typeof(DirectoryPlaceholder), resource);

                if (stream == null || stream.Length == 0)
                {
                    Console.WriteLine($"Error: Failed to load the resource stream for {resource}");
                    throw new InvalidOperationException($"Resource stream for {resource} is null or empty");
                }

                return stream;
            },
            Format = FakeBrowserVideoSourceFileFormat.MJpeg,
        };
}
