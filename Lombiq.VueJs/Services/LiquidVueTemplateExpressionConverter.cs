using Fluid;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Liquid;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class LiquidVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{
    private readonly ILiquidTemplateManager _liquidTemplateManager;

    public LiquidVueTemplateExpressionConverter(ILiquidTemplateManager liquidTemplateManager) =>
        _liquidTemplateManager = liquidTemplateManager;

    public bool IsApplicable(string name, string input, DisplayContext displayContext) =>
        "liquid".EqualsOrdinalIgnoreCase(name);

    public async ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        await _liquidTemplateManager.RenderStringAsync(
            input,
            NullEncoder.Default,
            displayContext);
}
