using System.ComponentModel.DataAnnotations;

namespace MvcDemo25.web.Models;

public class LoginModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    // Role to login as: Admin or User
    public string Role { get; set; } = "User";
}
