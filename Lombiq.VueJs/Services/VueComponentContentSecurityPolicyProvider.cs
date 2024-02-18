using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.VueJs.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.VueJs.Services;

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
