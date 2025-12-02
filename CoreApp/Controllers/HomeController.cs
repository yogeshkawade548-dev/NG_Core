using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoreApp.DataAccess;
using CoreApp.Models;

namespace CoreApp.Controllers;

public class HomeController : Controller
{
    private readonly LoginService _loginService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(LoginService loginService, ILogger<HomeController> logger)
    {
        _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        try
        {
            _logger.LogInformation("Index page accessed, redirecting to Login");
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Index action");
            return View("Error");
        }
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login([Bind("Username,Password")] LoginViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (_loginService.ValidateUser(model.Username, model.Password))
                {
                    return RedirectToAction("Dashboard");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            var sanitizedUsername = model?.Username?.Replace("\n", "").Replace("\r", "") ?? "unknown";
            _logger.LogError(ex, "Login attempt failed for user: {Username}", sanitizedUsername);
            ModelState.AddModelError("", "An error occurred during login. Please try again.");
        }
        return View(model);
    }

    public IActionResult Dashboard()
    {
        try
        {
            _logger.LogInformation("Dashboard accessed successfully");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing dashboard");
            return View("Error");
        }
    }

    public IActionResult ForgotPassword()
    {
        try
        {
            _logger.LogInformation("ForgotPassword page accessed");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing forgot password page");
            return View("Error");
        }
    }

    public IActionResult LayoutPage()
    {
        try
        {
            _logger.LogInformation("Layout page accessed");
            return View("Layout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing layout page");
            return View("Error");
        }
    }
}