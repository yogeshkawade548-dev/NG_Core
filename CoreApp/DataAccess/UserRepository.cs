using System;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using CoreApp.Models;
using CoreApp.Services;
using Microsoft.Extensions.Logging;

namespace CoreApp.DataAccess;

public class UserRepository
{
    private readonly string _connectionString;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(string connectionString, IPasswordService passwordService, ILogger<UserRepository> logger)
    {
        _connectionString = !string.IsNullOrEmpty(connectionString) 
            ? connectionString 
            : throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void CheckDatabase()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            var users = connection.Query<User>("SELECT Id, Username, Password FROM Users");
            _logger.LogInformation("Database check - Total users: {Count}", users.Count());
            
            foreach (var user in users)
            {
                _logger.LogInformation("User found - ID: {Id}, Username: {Username}", user.Id, user.Username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database");
        }
    }

    public void CreateDefaultUser()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            var existingUser = connection.QueryFirstOrDefault<User>(
                "SELECT Id FROM Users WHERE Username = @Username",
                new { Username = "admin" });
                
            if (existingUser == null)
            {
                var hashedPassword = _passwordService.HashPassword("admin");
                connection.Execute(
                    "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)",
                    new { Username = "admin", Password = hashedPassword });
                _logger.LogInformation("Default user created: admin/admin");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default user");
        }
    }

    public User? GetUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username cannot be null or empty", nameof(username));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty", nameof(password));
        
        var sanitizedUsername = username.Replace("\n", "").Replace("\r", "");
        
        try
        {
            _logger.LogDebug("Attempting to retrieve user: {Username}", sanitizedUsername);
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            var user = connection.QueryFirstOrDefault<User>(
                "SELECT Id, Username, Password FROM Users WHERE Username = @Username",
                new { Username = username });
                
            if (user != null)
            {
                _logger.LogDebug("User found, verifying password for: {Username}", sanitizedUsername);
                if (_passwordService.VerifyPassword(password, user.Password))
                {
                    _logger.LogInformation("User authentication successful: {Username}", sanitizedUsername);
                    return user;
                }
            }
            _logger.LogWarning("User authentication failed: {Username}", sanitizedUsername);
            return null;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving user: {Username}", sanitizedUsername);
            throw new InvalidOperationException("Database error occurred while retrieving user", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving user: {Username}", sanitizedUsername);
            throw new InvalidOperationException("An error occurred while retrieving user", ex);
        }
    }
}