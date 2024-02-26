using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Vue.js - Samples",
    Author = "Lombiq Technologies",
    Website = "https://github.com/Lombiq/Orchard-Vue.js",
    Version = "0.0.1",
    Description = "Samples for Lombiq Vue.js.",
    Category = "Vue.js",
    Dependencies = new[]
    {
        Lombiq.VueJs.Constants.FeatureIds.Area,
        "OrchardCore.ContentFields",
    }
)]
