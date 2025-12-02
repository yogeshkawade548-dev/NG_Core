using System;
using Microsoft.Extensions.Logging;

namespace CoreApp.Services;

public class PasswordService : IPasswordService
{
    private readonly ILogger<PasswordService> _logger;
    
    public PasswordService(ILogger<PasswordService> logger)
    {
        try
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        catch (ArgumentNullException ex)
        {
            throw new InvalidOperationException("Failed to initialize PasswordService", ex);
        }
    }
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }
        return password; // Simple storage without hashing
    }

    public bool VerifyPassword(string password, string storedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
        {
            return false;
        }
        return password == storedPassword;
    }
}