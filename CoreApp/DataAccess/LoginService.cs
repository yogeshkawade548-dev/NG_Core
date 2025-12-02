using System;
using System.Text.RegularExpressions;
using CoreApp.Models;
using Microsoft.Extensions.Logging;

namespace CoreApp.DataAccess;

public class LoginService
{
    private readonly UserRepository _userRepository;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserRepository userRepository, ILogger<LoginService> logger)
    {
        try
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize LoginService", ex);
        }
    }

    public bool ValidateUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("Validation failed: Username or password is null/empty");
            return false;
        }
            
        try
        {
            var user = _userRepository.GetUser(username, password);
            var isValid = user != null;
            _logger.LogInformation("User validation result for {Username}: {IsValid}", username, isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            var sanitizedUsername = System.Text.RegularExpressions.Regex.Replace(username ?? "unknown", @"[\r\n\t]", "");
            _logger.LogError(ex, "Error validating user: {Username}", sanitizedUsername);
            return false;
        }
    }
}