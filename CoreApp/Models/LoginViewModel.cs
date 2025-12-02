using System.ComponentModel.DataAnnotations;

namespace CoreApp.Models;

public class LoginViewModel
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username 
    { 
        get => _username; 
        set => _username = value ?? string.Empty; 
    }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password 
    { 
        get => _password; 
        set => _password = value ?? string.Empty; 
    }
}