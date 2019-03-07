using System.Collections.Generic;
using System.Linq;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;

namespace Lombiq.VueJs.Services
{
    public class VueComponentTemplateHarvester : IShapeTemplateHarvester
    {
        private const string SubPath = "Views/VueComponents";

        public IEnumerable<string> SubPaths() => new[] { SubPath };

        public IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info)
        {
            if (!info.SubPath.StartsWith(SubPath)) return Enumerable.Empty<HarvestShapeHit>();

            return new[]
            {
                new HarvestShapeHit
                {
                    ShapeType = "VueComponent-" +
                        info.FileName.Replace("--", "__").Replace("-", "__").Replace('.', '_').ToLowerInvariant()
                }
            };
        }
    }
}