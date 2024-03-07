using Lombiq.VueJs.Models;
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
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IResourceManager _resourceManager;

    [HtmlAttributeName("id")]
    public string Id { get; set; }

    [HtmlAttributeName("class")]
    public string Class { get; set; }

    [HtmlAttributeName("model")]
    public object Model { get; set; } = new { };


    [HtmlAttributeName("plugins")]
    public string Plugins { get; set; }

    public VueComponentAppTagHelper(
        IDisplayHelper displayHelper,
        IHttpContextAccessor hca,
        IOptions<ResourceManagementOptions> resourceManagementOptions,
        IResourceManager resourceManager,
        IShapeFactory shapeFactory,
        VueComponentTagHelperState state)
        : base(displayHelper, hca, resourceManagementOptions, resourceManager, shapeFactory, state) =>
        _resourceManager = resourceManager;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);
        _resourceManager.RegisterScriptModule(VueComponentApp);

        output.PostElement.AppendHtml(new TagBuilder("div")
        {
            Attributes =
            {
                ["id"] = string.IsNullOrWhiteSpace(Id) ? $"{Name}_{Guid.NewGuid():D}" : Id,
                ["class"] = $"{Class} lombiq-vue".Trim(),
                ["data-vue"] = JsonSerializer.Serialize(new { Name, Model }, _jsonSerializerOptions),
                ["data-plugins"] = Plugins ?? string.Empty,
            },
            TagRenderMode = TagRenderMode.Normal,
        });
    }
}
