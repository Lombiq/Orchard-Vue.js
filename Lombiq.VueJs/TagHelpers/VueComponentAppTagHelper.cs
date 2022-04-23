using Lombiq.VueJs.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OrchardCore.DisplayManagement;
using OrchardCore.ResourceManagement;
using System;
using System.Threading.Tasks;

namespace Lombiq.VueJs.TagHelpers;

[HtmlTargetElement("vue-component-app", Attributes = "area,name")]
public class VueComponentAppTagHelper : VueComponentTagHelper
{
    private readonly IDisplayHelper _displayHelper;
    private readonly IShapeFactory _shapeFactory;

    [HtmlAttributeName("id")]
    public string Id { get; set; }

    [HtmlAttributeName("class")]
    public string Class { get; set; }

    [HtmlAttributeName("model-property")]
    public string ModelProperty { get; set; } = "value";

    [HtmlAttributeName("model")]
    public object Model { get; set; } = new { };

    public VueComponentAppTagHelper(
        IDisplayHelper displayHelper,
        IOptions<ResourceManagementOptions> resourceManagementOptions,
        IResourceManager resourceManager,
        IShapeFactory shapeFactory)
        : base(displayHelper, resourceManagementOptions, resourceManager, shapeFactory)
    {
        _displayHelper = displayHelper;
        _shapeFactory = shapeFactory;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);

        if (string.IsNullOrWhiteSpace(Id)) Id = $"{Name}_{Guid.NewGuid():N}";

        var jObject = JObject.FromObject(
            Model,
            JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            }));
        var shapeModel = new VueComponentApp(jObject)
        {
            Id = Id,
            Class = Class,
            ModelProperty = ModelProperty,
            ComponentName = Name,
        };

        output.PostElement.AppendHtml(
            await _displayHelper.ShapeExecuteAsync(
                await _shapeFactory.CreateAsync("VueComponentApp", new { ShapeModel = shapeModel })));
    }
}
