using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcDemo25.web.Models;

public class Complaint
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    public string Description { get; set; }

    public ComplaintCategory Category { get; set; }

    public ComplaintPriority Priority { get; set; }

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Link to owning user: store User.Id as string for simplicity
    public string? UserId { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();
}

