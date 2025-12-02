using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CoreApp.DataAccess;
using CoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=Core_App_DB;Trusted_Connection=true;";
    
    builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Information);
    
    builder.Services.AddScoped<IPasswordService, PasswordService>();
    builder.Services.AddScoped<UserRepository>(provider => 
        new UserRepository(connectionString, 
            provider.GetRequiredService<IPasswordService>(),
            provider.GetRequiredService<ILogger<UserRepository>>()));
    builder.Services.AddScoped<LoginService>();
    builder.Services.AddControllersWithViews();
    builder.Services.AddAntiforgery();

    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application starting up");
    
    // Create default user if not exists
    using (var scope = app.Services.CreateScope())
    {
        var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();
        userRepo.CheckDatabase();
        userRepo.CreateDefaultUser();
        userRepo.CheckDatabase();
    }

    app.UseStaticFiles();
    app.UseRouting();

    app.MapGet("/", () => Results.Redirect("/Home/Login"));

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Login}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    var logger = loggerFactory.CreateLogger("Program");
    logger.LogCritical(ex, "Application startup failed with error: {ErrorMessage}", ex.Message);
    throw;
}