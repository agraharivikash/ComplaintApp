using System.ComponentModel.DataAnnotations;

namespace MvcDemo25.web.Models;

public class RegisterModel
{
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
    // Role to register as: "User" or "Admin". Default is "User".
    public string? Role { get; set; }
}
