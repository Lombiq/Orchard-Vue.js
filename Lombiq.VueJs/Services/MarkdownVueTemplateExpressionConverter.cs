using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class MarkdownVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{

    public bool IsApplicable(string name, string input, DisplayContext displayContext) =>
        "markdown".EqualsOrdinalIgnoreCase(name);

    public ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        throw new NotImplementedException();
}
