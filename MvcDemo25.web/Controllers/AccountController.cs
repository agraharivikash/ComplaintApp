using Microsoft.AspNetCore.Mvc;
using MvcDemo25.web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MvcDemo25.web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _ctx;

    public AccountController(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _ctx.Users.AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email))
        {
            ModelState.AddModelError(string.Empty, "User with same username or email already exists");
            return View(model);
        }

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password),
            Role = model.Role ?? "User"
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        await SignInUser(user);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _ctx.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName && u.Role == model.Role);
        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials");
            return View(model);
        }

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // include role claim
        if (!string.IsNullOrEmpty(user.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var hashed = HashPassword(password);
        return hashed == hash;
    }
}
