using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo25.web.Models;

namespace MvcDemo25.web.Controllers;

public class ComplaintsController : Controller
{
    private readonly AppDbContext _db;

    public ComplaintsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, ComplaintStatus? status, ComplaintPriority? priority)
    {
        var query = _db.Complaints.Include(c => c.Comments).AsQueryable();
        // If not admin, restrict to user's complaints
        if (!(User?.IsInRole("Admin") ?? false))
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            query = query.Where(c => c.UserId == userId);
        }
        if (!string.IsNullOrEmpty(q)) query = query.Where(c => c.Title.Contains(q) || c.Description.Contains(q));
        if (status != null) query = query.Where(c => c.Status == status);
        if (priority != null) query = query.Where(c => c.Priority == priority);
        var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(list);
    }

    public IActionResult Create()
    {
        if (!(User?.Identity?.IsAuthenticated ?? false)) return Challenge();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Complaint model)
    {
        if (!ModelState.IsValid) return View(model);
        // Must be authenticated
        if (!(User?.Identity?.IsAuthenticated ?? false)) return Challenge();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        model.UserId = userId;
        _db.Complaints.Add(model);
        await _db.SaveChangesAsync();
        TempData["success"] = "Complaint registered.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await _db.Complaints.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return NotFound();
        // Authorization: owner or admin can view
        var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!(User?.IsInRole("Admin") ?? false) && c.UserId != userId) return Forbid();
        return View(c);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int complaintId, string text)
    {
        if (!(User?.Identity?.IsAuthenticated ?? false)) return Challenge();

        var c = await _db.Complaints.FindAsync(complaintId);
        if (c == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        // Only owner or admin can comment
        if (!(User?.IsInRole("Admin") ?? false) && c.UserId != userId) return Forbid();

        var comment = new Comment { ComplaintId = complaintId, Text = text, UserId = userId };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return RedirectToAction("Details", new { id = complaintId });
    }
}
