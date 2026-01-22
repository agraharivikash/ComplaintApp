using System.ComponentModel.DataAnnotations;

namespace MvcDemo25.web.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ComplaintId { get; set; }
    public Complaint Complaint { get; set; }

    public string? UserId { get; set; }
}
