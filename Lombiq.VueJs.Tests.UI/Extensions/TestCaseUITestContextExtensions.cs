using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Tests.UI.Extensions
{
    public static class TestCaseUITestContextExtensions
    {
        public static async Task TestVueSampleBehaviorAsync(this UITestContext context)
        {
            await context.SignInDirectlyAsync();

            await context.TestVueAppAsync();
            await context.TestVueSfcASync();
            await context.TestVueSfcEnhancedListAsync();
        }

        public static async Task TestVueAppAsync(this UITestContext context)
        {
        }

        public static async Task TestVueSfcASync(this UITestContext context)
        {
        }

        public static async Task TestVueSfcEnhancedListAsync(this UITestContext context)
        {
        }
    }
}
