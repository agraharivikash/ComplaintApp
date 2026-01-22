using System.ComponentModel.DataAnnotations;

namespace MvcDemo25.web.Models;

public class Person
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string LastName { get; set; } = string.Empty;

    // Owner of the person record (nullable to allow existing rows without an owner)
    public int? UserId { get; set; }

    public User? User { get; set; }

}
