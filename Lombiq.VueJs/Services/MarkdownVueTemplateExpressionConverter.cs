using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Markdown.Services;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class MarkdownVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{
    private readonly IMarkdownService _markdownService;

    public MarkdownVueTemplateExpressionConverter(IMarkdownService markdownService) =>
        _markdownService = markdownService;

    public bool IsApplicable(string name, string input, DisplayContext displayContext) =>
        "markdown".EqualsOrdinalIgnoreCase(name);

    public ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        ValueTask.FromResult(_markdownService.ToHtml(input));
}
