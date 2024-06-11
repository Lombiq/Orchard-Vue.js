using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.Mvc.Utilities;
using System;
using System.Collections.Generic;

namespace Lombiq.VueJs.Services;

public class VueSingleFileComponentTemplateHarvester : IShapeTemplateHarvester
{
    private const string SubPath = "Assets/Scripts/VueComponents";

    public IEnumerable<string> SubPaths() => [SubPath];

    public IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info) =>
        !info.SubPath.StartsWith(SubPath, StringComparison.OrdinalIgnoreCase)
            ? []
            :
            [
                new HarvestShapeHit
                {
                    ShapeType = "VueComponent-" + info.FileName.ToPascalCaseDash(),
                },
            ];
}
