using Microsoft.EntityFrameworkCore;

namespace MvcDemo25.web.Models;

public class AppDbContext:DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {

    }
    // Keep existing People and Users for compatibility
    public DbSet<Person> People { get; set; }
    public DbSet<User> Users { get; set; }
    // New entities for Complaint & Issue Tracking
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<Comment> Comments { get; set; }
}
