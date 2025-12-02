using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Areas.Inventory.Controllers;

[Area("Inventory")]
public class UpdateStockController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}