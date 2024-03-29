using Lombiq.VueJs.Constants;
using Lombiq.VueJs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement;
using OrchardCore.Mvc.Utilities;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.VueJs.TagHelpers;

[HtmlTargetElement("vue-component", Attributes = "name")]
public class VueComponentTagHelper : TagHelper
{
    private readonly IDisplayHelper _displayHelper;
    private readonly IHttpContextAccessor _hca;
    private readonly IOptions<ResourceManagementOptions> _resourceManagementOptions;
    private readonly IResourceManager _resourceManager;
    private readonly IShapeFactory _shapeFactory;
    private readonly VueComponentTagHelperState _state;

    [HtmlAttributeName("area")]
    public string Area { get; set; }

    [HtmlAttributeName("name")]
    public string Name { get; set; }

    [HtmlAttributeName("children")]
    public string Children { get; set; }

    public VueComponentTagHelper(
        IDisplayHelper displayHelper,
        IHttpContextAccessor hca,
        IOptions<ResourceManagementOptions> resourceManagementOptions,
        IResourceManager resourceManager,
        IShapeFactory shapeFactory,
        VueComponentTagHelperState state)
    {
        _displayHelper = displayHelper;
        _hca = hca;
        _resourceManagementOptions = resourceManagementOptions;
        _resourceManager = resourceManager;
        _shapeFactory = shapeFactory;
        _state = state;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var area = string.IsNullOrEmpty(Area) ? _hca.HttpContext?.Request.RouteValues.GetMaybe("area")?.ToString() : Area;
        if (string.IsNullOrEmpty(area))
        {
            throw new InvalidOperationException(
                $"Failed to automatically resolve the current area name. Please specify the correct value in the " +
                $"\"area\" attribute of your <{context.TagName}> tag helper. This is usually the name of the module " +
                $"where the vue file is located.");
        }

        _resourceManager
            .InlineManifest
            .DefineScriptModule(Name + ".vue")
            .SetUrl($"~/{area}/vue/{Name}.min.js", $"~/{area}/vue/{Name}.js");

        foreach (var resourceName in FindResourceNames())
        {
            var shapeType = "VueComponent-" + resourceName.ToPascalCaseDash();

            output.PostElement.AppendHtml(
                await _displayHelper.ShapeExecuteAsync(
                    await _shapeFactory.CreateAsync(shapeType)));
        }

        _state.Active = true;
        output.TagName = null;
    }

    private HashSet<string> FindResourceNames()
    {
        var resourceNames = (Children?.Split(',') ?? Enumerable.Empty<string>())
            .SelectWhere(child => child.Trim(), child => !string.IsNullOrEmpty(child))
            .ToHashSet();
        resourceNames.Add(Name);

        // The key is the resource name and the value is one of its dependencies.
        var componentDependencies = _resourceManagementOptions
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
