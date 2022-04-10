using Lombiq.VueJs.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Lombiq.VueJs.Samples.Controllers;

// This controller is for returning a Vue.js Single File Component (abbreviated as SFC going forward) in an MVC view.
// However, you could use any other ways of doing it such as injecting as a shape or using widgets.
public class VueSfcController : Controller
{
    private readonly IClock _clock;
    private readonly IStringLocalizer<VueSfcController> T;

    public VueSfcController(IClock clock, IStringLocalizer<VueSfcController> stringLocalizer)
    {
        _clock = clock;
        T = stringLocalizer;
    }

    [HttpGet]
    // Open this from under /Lombiq.VueJs.Samples/VueSfc/Index
    public ActionResult Index() => View();

    [HttpGet]
    // Open this from under /Lombiq.VueJs.Samples/VueSfc/AppTagHelper
    public ActionResult AppTagHelper() => View();

    // NEXT STATION: Views/VueSfc/Index.cshtml

    // Here we show off a way of doing progressive enhancement. This first action has everything you need to access the
    // page without JS.
    // Open this from under /Lombiq.VueJs.Samples/VueSfc/EnhancedList
    public ActionResult EnhancedList(int page = 1) => View(new EnhancedListViewModel
    {
        Page = page,
        Data = GetDataForPage(page),
    });

    // This method returns JSON, providing an API equivalent for the EnhancedList action. The enhanced app will use that
    // to update results asynchronously. Both use the same source of data.
    public ActionResult GetList(int page = 1) => Json(GetDataForPage(page));

    // What is in this method isn't really important, just some sample data to show change.
    [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "It's not security critical.")]
    [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "Same.")]
    private IEnumerable<EnhancedListViewModel.EnhancedListViewModelData> GetDataForPage(int page)
    {
        var today = _clock.UtcNow;

        return Enumerable.Range((page - 1) * 10, 10)
            .Select(index => new EnhancedListViewModel.EnhancedListViewModelData
            {
                Number = index + 1,
                Date = T["{0:d}", today.AddDays(index)],
                Day = T["{0:dddd}", today.AddDays(index)].Value,
                Random = new Random(index).Next(10) + 1, // Not actually random, more like deterministic noise.
            });
    }

    // NEXT STATION: Views/VueSfc/EnhancedList.cshtml
}
