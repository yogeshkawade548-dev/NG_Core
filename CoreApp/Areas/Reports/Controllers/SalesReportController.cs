using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Areas.Reports.Controllers;

[Area("Reports")]
public class SalesReportController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}