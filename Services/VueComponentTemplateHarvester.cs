using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.VueJs.Services
{
    public class VueComponentTemplateHarvester : IShapeTemplateHarvester
    {
        private const string SubPath = "Views/VueComponents";

        public IEnumerable<string> SubPaths() => new[] { SubPath };

        public IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info) =>
            !info.SubPath.StartsWith(SubPath, StringComparison.OrdinalIgnoreCase)
                ? Enumerable.Empty<HarvestShapeHit>()
                : new[]
                {
                    new HarvestShapeHit
                    {
                        ShapeType = "VueComponent-" + info.FileName
                            .Replace("--", "__", StringComparison.OrdinalIgnoreCase)
                            .Replace("-", "__", StringComparison.OrdinalIgnoreCase)
                            .Replace(".", "_", StringComparison.OrdinalIgnoreCase)
                            .ToUpperInvariant(),
                    },
                };
    }
}
