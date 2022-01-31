using Lombiq.VueJs.Constants;
using Microsoft.AspNetCore.Razor.TagHelpers;
using OrchardCore.ResourceManagement;

namespace Lombiq.VueJs.TagHelpers
{
    [HtmlTargetElement("vue-component", Attributes = "area,name")]
    public class VueComponentTagHelper : TagHelper
    {
        private readonly IResourceManager _resourceManager;

        [HtmlAttributeName("area")]
        public string Area { get; set; }

        [HtmlAttributeName("name")]
        public string Name { get; set; }

        public VueComponentTagHelper(IResourceManager resourceManager) => _resourceManager = resourceManager;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _resourceManager.RegisterResource("script", ResourceNames.Vue).AtHead();

            var scriptName = "vue-component-" + Name;
            _resourceManager.InlineManifest
                .DefineScript(scriptName)
                .SetUrl($"/{Area}/vue/{Name}.min.js", $"/{Area}/vue/{Name}.js")
                .SetDependencies(ResourceNames.Vue);
            _resourceManager.RegisterScript(scriptName).AtFoot();

            output.SuppressOutput();
        }
    }
}
