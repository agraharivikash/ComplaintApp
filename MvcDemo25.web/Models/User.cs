using System.ComponentModel.DataAnnotations;

namespace MvcDemo25.web.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Role of the user. Use "User" or "Admin".
    public string Role { get; set; } = "User";
}
