using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;
using static Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement.ResourceTypes;
using static Lombiq.VueJs.Constants.ResourceNames;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Enable the <see cref="UnsafeEval"/> value for the <see cref="ScriptSrc"/> directive when the <see cref="Vue3" />
/// resource is present. This is necessary to evaluate dynamic (not precompiled) templates used by this module.
/// </summary>
public class Vue3ContentSecurityPolicyProvider : ResourceManagerContentSecurityPolicyProvider
{
    protected override string ResourceType => ScriptModule;
    protected override string ResourceName => Vue3;
    protected override IReadOnlyCollection<string> DirectiveNameChain { get; } = new[] { ScriptSrc };
    protected override string DirectiveValue => UnsafeEval;
}
