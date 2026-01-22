using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo25.web.Models;
using System;

namespace MvcDemo25.Web.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
public class PersonController : Controller
{
    private readonly ILogger<PersonController> _logger;
    private readonly AppDbContext _ctx;

    public PersonController(ILogger<PersonController> logger, AppDbContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var people = await _ctx
                .People
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.FirstName)
                .ThenByDescending(p => p.LastName)
                .ToListAsync();
            return View(people);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = "Error loading Person list.";
            // Return an empty list so the view that expects a model does not throw
            return View(Enumerable.Empty<Person>());
        }

    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Person person)
    {
        if (!ModelState.IsValid)
        {
            return View(person);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            person.UserId = userId;
            _ctx.People.Add(person);  // person object is added to context
            await _ctx.SaveChangesAsync(); // actual database call
            TempData["success"] = "Saved successfully!";
            return RedirectToAction(nameof(Add));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = "Error on saving Person record!";
            return View(person);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var person = await _ctx.People.SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (person == null)
            {
                TempData["error"] = "No record found";
                return RedirectToAction(nameof(Index));
            }

            return View(person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = "Something went wrong!";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Person person)
    {
        if (!ModelState.IsValid)
        {
            return View(person);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (!await _ctx.People.AnyAsync(p => p.Id == person.Id && p.UserId == userId))
            {
                TempData["error"] = "No record found";
                return View(person);
            }

            // Ensure we don't change ownership via the edit
            person.UserId = userId;
            _ctx.People.Update(person);  // person object is added to context
            await _ctx.SaveChangesAsync(); // actual database call
            TempData["success"] = "Updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = "Error on saving Person record!";
            return View(person);
        }
    }


    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var person = await _ctx.People.SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (person == null)
            {
                TempData["error"] = "No record found";
                return RedirectToAction(nameof(Index));
            }

            _ctx.People.Remove(person);
            await _ctx.SaveChangesAsync();
            TempData["success"] = "Deleted successfull!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = "Something went wrong!";
        }
        return RedirectToAction(nameof(Index));
    }
}