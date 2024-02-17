using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs.TagHelpers;

[HtmlTargetElement("vue-component-app", Attributes = "name")]
public class VueComponentAppTagHelper : VueComponentTagHelper
{
    private readonly IResourceManager _resourceManager;

    [HtmlAttributeName("id")]
    public string Id { get; set; }

    [HtmlAttributeName("class")]
    public string Class { get; set; }

    [HtmlAttributeName("model-property")]
    public IEnumerable<string> ModelProperties { get; set; } = new[] { "value", "modelValue" };

    [HtmlAttributeName("model")]
    public object Model { get; set; } = new { };

    public VueComponentAppTagHelper(
        IDisplayHelper displayHelper,
        IHttpContextAccessor hca,
        IOptions<ResourceManagementOptions> resourceManagementOptions,
        IResourceManager resourceManager,
        IShapeFactory shapeFactory)
        : base(displayHelper, hca, resourceManagementOptions, resourceManager, shapeFactory) =>
        _resourceManager = resourceManager;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);
        _resourceManager.RegisterScriptModule(VueComponentApp);

        var dataVue = new { Name, Model, ModelProperties };
        output.PostElement.AppendHtml(new TagBuilder("div")
        {
            Attributes =
            {
                ["id"] = string.IsNullOrWhiteSpace(Id) ? $"{Name}_{Guid.NewGuid():D}" : Id,
                ["class"] = $"{Class} lombiq-vue".Trim(),
                ["data-vue"] = Json(dataVue),
            },
            TagRenderMode = TagRenderMode.Normal,
        });
    }

    private static string Json<T>(T value) =>
        JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
}
