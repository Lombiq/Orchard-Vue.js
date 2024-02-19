using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.VueJs.Models;
using Lombiq.VueJs.TagHelpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.VueJs.Services;

/// <summary>
/// Enable the <see cref="UnsafeEval"/> value for the <see cref="ScriptSrc"/> when <see cref="VueComponentTagHelper"/>
/// or <see cref="VueComponentAppTagHelper"/> are used. This is necessary to evaluate dynamic (not precompiled)
/// templates used by this module.
/// </summary>
public class VueComponentContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private readonly VueComponentTagHelperState _state;

    public VueComponentContentSecurityPolicyProvider(VueComponentTagHelperState state) => _state = state;

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (_state.Active)
        {
            IContentSecurityPolicyProvider.MergeDirectiveValues(securityPolicies, new[] { ScriptSrc }, UnsafeEval);
        }

        return ValueTask.CompletedTask;
    }
}
