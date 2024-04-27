using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class LiquidVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{

    public bool IsApplicable(string name, string input, DisplayContext displayContext) =>
        "liquid".EqualsOrdinalIgnoreCase(name);

    public ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        throw new NotImplementedException();
}
