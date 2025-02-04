using Lombiq.VueJs.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Services;

public class StringLocalizerVueTemplateExpressionConverter : IVueTemplateExpressionConverter
{
    private readonly Lazy<IStringLocalizer> _stringLocalizerLazy;

    public IStringLocalizer StringLocalizer => _stringLocalizerLazy.Value;

    public string Name => TemplateSegment.StringLocalizerConverterName;

    internal StringLocalizerVueTemplateExpressionConverter(Lazy<IStringLocalizer> stringLocalizerLazy) =>
        _stringLocalizerLazy = stringLocalizerLazy;

    public ValueTask<string> ConvertAsync(string name, string input, DisplayContext displayContext) =>
        ValueTask.FromResult(WebUtility.HtmlEncode(_stringLocalizerLazy.Value[input]));
}
