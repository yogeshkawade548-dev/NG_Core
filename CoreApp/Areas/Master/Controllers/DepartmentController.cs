using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Areas.Master.Controllers;

[Area("Master")]
public class DepartmentController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}