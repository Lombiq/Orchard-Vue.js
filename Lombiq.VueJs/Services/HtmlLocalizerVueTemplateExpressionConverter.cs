using Lombiq.VueJs.Models;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class HtmlLocalizerVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{
    private readonly Lazy<IHtmlLocalizer> _htmlLocalizerLazy;

    internal HtmlLocalizerVueTemplateExpressionConverter(Lazy<IHtmlLocalizer> htmlLocalizerLazy) =>
        _htmlLocalizerLazy = htmlLocalizerLazy;

    public bool IsApplicable(string name, string input, DisplayContext displayContext) =>
        TemplateSegment.StringLocalizerConverterName.EqualsOrdinalIgnoreCase(name);

    public ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        ValueTask.FromResult(_htmlLocalizerLazy.Value[input].Html());
}
