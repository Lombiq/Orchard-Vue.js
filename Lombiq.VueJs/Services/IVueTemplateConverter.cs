using OrchardCore.DisplayManagement.Implementation;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

/// <summary>
/// A service that handles <c>[[{name} input ]]</c> expressions in Vue SFC templates. Used by <see
/// cref="VueSingleFileComponentShapeTemplateViewEngine"/>.d
/// </summary>
public interface IVueTemplateConverter
{
    /// <summary>
    /// Returns a value indicating whether this converter should handle the provided <paramref name="input"/>, typically
    /// based on the <paramref name="name"/>.
    /// </summary>
    bool IsApplicable(string name, string input, DisplayContext displayContext);

    /// <summary>
    /// Returns the output that should be substituted instead of the provided expression.
    /// </summary>
    ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext);
}
