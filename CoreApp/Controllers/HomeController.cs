using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoreApp.DataAccess;
using CoreApp.Models;
using Microsoft.AspNetCore.Http;
using CoreApp.Services;

namespace CoreApp.Controllers;

public class HomeController : Controller
{
    private readonly LoginService _loginService;
    private readonly ILogger<HomeController> _logger;
    private readonly IJwtService _jwtService;

    public HomeController(LoginService loginService, ILogger<HomeController> logger, IJwtService jwtService)
    {
        _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }

    public IActionResult Index()
    {
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login([Bind("Username,Password")] LoginViewModel model)
    {
        if (ModelState.IsValid && _loginService.ValidateUser(model.Username, model.Password))
        {
            var token = _jwtService.GenerateToken(model.Username);
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(24)
            });
            return RedirectToAction("Dashboard");
        }
        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
    }

    [JwtAuthorize]
    public IActionResult Dashboard()
    {
        ViewBag.Username = HttpContext.Items["Username"]?.ToString();
        return View();
    }

    [JwtAuthorize]
    public IActionResult LayoutPage()
    {
        ViewBag.Username = HttpContext.Items["Username"]?.ToString();
        return View("Layout");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt_token");
        return RedirectToAction("Login");
    }
}