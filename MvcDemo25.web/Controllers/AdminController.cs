using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo25.web.Models;

namespace MvcDemo25.web.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!(User?.IsInRole("Admin") ?? false)) return Forbid();
        var complaints = await _db.Complaints.Include(c => c.Comments).OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(complaints);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, ComplaintStatus status)
    {
        if (!(User?.IsInRole("Admin") ?? false)) return Forbid();
        var c = await _db.Complaints.FindAsync(id);
        if (c == null) return NotFound();
        c.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction("Dashboard");
    }
}
