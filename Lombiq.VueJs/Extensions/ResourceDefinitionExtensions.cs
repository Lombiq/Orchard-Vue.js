using System;

namespace OrchardCore.ResourceManagement;

public static class ResourceDefinitionExtensions
{
    public static ResourceDefinition SetExternalVueComponent(
        this ResourceDefinition manifest,
        string resourceName,
        string javascriptName = null)
    {
        ArgumentNullException.ThrowIfNull(resourceName);
        if (string.IsNullOrEmpty(javascriptName)) javascriptName = resourceName;

        if (resourceName.Contains(':') || javascriptName.Contains(':'))
        {
            throw new InvalidOperationException("The external vue component names must not contain colon characters.");
        }

        return manifest.SetDependencies($"script:{resourceName}:{javascriptName}");
    }
}
