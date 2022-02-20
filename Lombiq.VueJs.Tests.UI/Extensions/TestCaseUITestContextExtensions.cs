using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Globalization;
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
            await context.GoToVueAppAsync();

            var byText = By.ClassName("demo__text");
            var byButton = By.ClassName("demo__showHideButton");

            context.Missing(byText);

            context.ClickReliablyOn(byButton);
            context.Exists(byText);

            context.ClickReliablyOn(byButton);
            context.Missing(byText);
        }

        public static async Task TestVueSfcASync(this UITestContext context)
        {
            await context.GoToVueSfcAsync();

            var byItem = By.ClassName("DemoRepeater__listItem");

            // Test the random button.
            for (var i = 0; i < 10; i++)
            {
                context.ClickReliablyOn(By.Id("random"));

                var count = int.Parse(
                    context.Get(By.Id("demoApp")).GetAttribute("data-count"),
                    CultureInfo.InvariantCulture);

                context.GetAll(byItem).Count.ShouldBe(count);
            }

            // We need an ID for the dropdown.
            context.ExecuteScript("document.querySelector('.DemoSfc__select').id = 'dropdown';");

            // Go through all dropdown options.
            for (var number = (int)Numeric.One; number <= 10; number++)
            {
                context.SetDropdown("dropdown", (Numeric)number);

                context.GetAll(byItem).Count.ShouldBe(number);
            }

            // Verify localizer HTML escaping.
            context.Missing(By.ClassName("not-html"));
        }

        public static async Task TestVueSfcEnhancedListAsync(this UITestContext context)
        {
            await context.GoToVueSfcEnhancedListAsync();

            var byFirstRow = By.CssSelector("tbody tr:first-child td");
            var byPreviousButton = By.CssSelector("#controls button:first-child");

            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var today = DateTime.UtcNow;

            void VerifyFirstPageFirstRow() =>
                context.VerifyElementTexts(
                    byFirstRow,
                    1,
                    string.Format(culture, "{0:d}", today),
                    string.Format(culture, "{0:dddd}", today),
                    "★★★★★★★★✰✰");

            // Verify initial state.
            VerifyFirstPageFirstRow();

            // Check pager behavior.
            context.Get(byPreviousButton).GetAttribute("hidden").ShouldBe("hidden");
            context.ClickReliablyOn(By.CssSelector("#controls button:last-child"));
            context.Get(byPreviousButton).GetAttribute("hidden").ShouldBe(expected: null);

            // Verify page change effect in table.
            var later = today.AddDays(10);
            context.VerifyElementTexts(
                byFirstRow,
                11,
                string.Format(culture, "{0:d}", later),
                string.Format(culture, "{0:dddd}", later),
                "★★★★★★★★★★");

            context.ClickReliablyOn(byPreviousButton);
            VerifyFirstPageFirstRow();
        }

        private enum Numeric { One = 1 }
    }
}
