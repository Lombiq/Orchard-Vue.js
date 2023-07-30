using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.Services;
using Lombiq.VueJs.Tests.UI.Assets.Media;

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
            StreamProvider = () => typeof(DirectoryPlaceholder)
                .Assembly
                .GetManifestResourceStream(typeof(DirectoryPlaceholder), resource),
            Format = FakeBrowserVideoSourceFileFormat.MJpeg,
        };
}
