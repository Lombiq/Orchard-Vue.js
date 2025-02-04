namespace Lombiq.VueJs.Models;

public record TemplateSegment(string Value, string ConverterName = null, bool IsLocalizable = true)
{
    public const string HtmlLocalizerConverterName = "html";
    public const string StringLocalizerConverterName = "string";

    public static TemplateSegment NonLocalizable(string value) => new(value, IsLocalizable: false);
    public static TemplateSegment HtmlLocalizer(string value) => new(value, HtmlLocalizerConverterName);
    public static TemplateSegment StringLocalizer(string value) => new(value, StringLocalizerConverterName);
}
