using Lombiq.VueJs.Constants;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement;
using OrchardCore.Mvc.Utilities;
using OrchardCore.ResourceManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.VueJs.TagHelpers;

[HtmlTargetElement("vue-component", Attributes = "area,name")]
public class VueComponentTagHelper(
    IDisplayHelper displayHelper,
    IOptions<ResourceManagementOptions> resourceManagementOptions,
    IResourceManager resourceManager,
    IShapeFactory shapeFactory) : TagHelper
{
    [HtmlAttributeName("area")]
    public string Area { get; set; }

    [HtmlAttributeName("name")]
    public string Name { get; set; }

    [HtmlAttributeName("children")]
    public string Children { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        resourceManager.RegisterResource("script", ResourceNames.Vue).AtHead();

        var scriptName = "vue-component-" + Name;
        resourceManager.InlineManifest
            .DefineScript(scriptName)
            .SetUrl($"~/{Area}/vue/{Name}.min.js", $"~/{Area}/vue/{Name}.js")
            .SetDependencies(ResourceNames.Vue);
        resourceManager.RegisterScript(scriptName).AtFoot();

        foreach (var resourceName in FindResourceNames())
        {
            var shapeType = "VueComponent-" + resourceName.ToPascalCaseDash();

            output.PostElement.AppendHtml(
                await displayHelper.ShapeExecuteAsync(
                    await shapeFactory.CreateAsync(shapeType)));
        }

        output.TagName = null;
    }

    private HashSet<string> FindResourceNames()
    {
        var resourceNames = (Children?.Split(',') ?? Enumerable.Empty<string>())
            .SelectWhere(child => child.Trim(), child => !string.IsNullOrEmpty(child))
            .ToHashSet();
        resourceNames.Add(Name);

        // The key is the resource name and the value is one of its dependencies.
        var componentDependencies = resourceManagementOptions
                .Value
                .ResourceManifests
                .SingleResourceTypeToLookup(ResourceTypes.SingleFileComponent);

        AddShapesRecursively(resourceNames, resourceNames, componentDependencies);

        return resourceNames;
    }

    private static void AddShapesRecursively(
        ISet<string> resourceNames,
        IEnumerable<string> resourcesToCheck,
        ILookup<string, string> componentDependencies)
    {
        var newDependencies = resourcesToCheck
            .SelectMany(resource => componentDependencies[resource])
            .Where(dependency => !resourceNames.Contains(dependency))
            .ToHashSet();

        if (newDependencies.Count == 0) return;

        resourceNames.AddRange(newDependencies);
        AddShapesRecursively(resourceNames, newDependencies, componentDependencies);
    }
}
