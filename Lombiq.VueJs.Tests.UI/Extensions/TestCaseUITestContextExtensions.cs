using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Lombiq.VueJs.Tests.UI.Extensions;

public static class TestCaseUITestContextExtensions
{
    public static async Task TestVueSampleBehaviorAsync(this UITestContext context)
    {
        await context.TestVueAppAsync();
        await context.TestVueSfcASync(withAppTagHelper: false);
        await context.TestVueSfcASync(withAppTagHelper: true);
        await context.TestVueSfcEnhancedListAsync();
    }

    public static async Task TestVueAppAsync(this UITestContext context)
    {
        await context.GoToVueAppAsync();

        var byText = By.ClassName("demo__text");
        var byButton = By.ClassName("demo__showHideButton");

        context.Missing(byText);

        await context.ClickReliablyOnAsync(byButton);
        context.Exists(byText);

        await context.ClickReliablyOnAsync(byButton);
        context.Missing(byText);
    }

    public static async Task TestVueSfcASync(this UITestContext context, bool withAppTagHelper)
    {
        await (withAppTagHelper ? context.GoToVueSfcAppTagHelperAsync() : context.GoToVueSfcAsync());

        var byItem = By.ClassName("DemoRepeater__listItem");

        // Test the "Pick Random!" button.
        for (var i = 0; i < 10; i++)
        {
            await context.ClickReliablyOnAsync(By.Id("random"));

            var count = int.Parse(
                context.Get(By.Id(withAppTagHelper ? "unique-id" : "demoApp")).GetAttribute("data-count"),
                CultureInfo.InvariantCulture);

            context.GetAll(byItem).Count.ShouldBe(count);
        }

        // We need an ID for the dropdown.
        context.ExecuteScript("document.querySelector('.DemoSfc__select').id = 'dropdown';");

        // Go through all dropdown options.
        for (var number = (int)Numeric.One; number <= 10; number++)
        {
            await context.SetDropdownAsync("dropdown", (Numeric)number);

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
        context.Get(byPreviousButton).GetAttribute("class").Split().ShouldContain("disabled");
        await context.ClickReliablyOnAsync(By.CssSelector("#controls button:last-child"));
        context.Get(byPreviousButton).GetAttribute("class").Split().ShouldNotContain("disabled");

        // Verify page change effect in table.
        var later = today.AddDays(10);
        context.VerifyElementTexts(
            byFirstRow,
            11,
            string.Format(culture, "{0:d}", later),
            string.Format(culture, "{0:dddd}", later),
            "★★★★★★★★★★");

        await context.ClickReliablyOnAsync(byPreviousButton);
        VerifyFirstPageFirstRow();
    }

    public static async Task TestQrCardFoundAsync(this UITestContext context)
    {
        await context.SetupAndNavigateQrCardAppAsync();
        context.WaitForCardElementAndAssert(
            By.CssSelector(".qr-card .full-name"),
            element => element.Text.ShouldBe("John Doe"));
    }

    public static async Task TestQrCardNotFoundAsync(this UITestContext context)
    {
        await context.SetupAndNavigateQrCardAppAsync();
        context.WaitForCardElementAndAssert(
            By.CssSelector(".qr-card .message-error"),
            element => element.Text.ShouldContain("404"));
    }

    private static async Task SetupAndNavigateQrCardAppAsync(this UITestContext context)
    {
        await context.ExecuteQrCardSampleRecipeDirectlyAsync();
        context.SetViewportSize(CommonDisplayResolutions.WxgaPlus);
        await context.GoToQrCardAppAsync();
    }

    private static void WaitForCardElementAndAssert(
        this UITestContext context,
        By elementToWaitSelector,
        Action<IWebElement> assert) =>
        context.DoWithRetriesOrFail(
            () =>
            {
                if (context.Exists(elementToWaitSelector))
                {
                    assert(context.Get(elementToWaitSelector));

                    return true;
                }

                return false;
            });

    private enum Numeric { One = 1 }
}
