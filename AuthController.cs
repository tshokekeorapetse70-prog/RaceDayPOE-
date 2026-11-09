using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RaceDayContext db) : ControllerBase
{
    private readonly PasswordHasher<UserAccount> hasher = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var role = request.Role.Trim();

        if (role is not ("Organiser" or "Participant"))
            return BadRequest("Role must be Organiser or Participant.");

        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.UserAccounts.AnyAsync(x => x.Email == email))
            return Conflict("Email already exists.");

        var user = new UserAccount
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = role
        };

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        db.UserAccounts.Add(user);
        await db.SaveChangesAsync();

        return Created("/api/auth/me", new AuthResponse(
            user.UserAccountId,
            user.FullName,
            user.Email,
            user.Role));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await db.UserAccounts
            .SingleOrDefaultAsync(x => x.Email == email);

        if (user is null ||
            hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,
                user.UserAccountId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Ok(new AuthResponse(
            user.UserAccountId,
            user.FullName,
            user.Email,
            user.Role));
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = User.Identity?.Name,
            Email = User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(new { message = "Logged out successfully." });
    }
}