using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Areas.Master.Controllers;

[Area("Master")]
public class RoleManagementController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}