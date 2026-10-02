using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHire.Data;
using SmartHire.Entities;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LocalLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly SmartHireDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(SmartHireDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Registers a new local (username/password) user account and grants RESUME_AI access.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "User, Email and Password are required." });
        }

        var usernameExists = await _db.Users.AnyAsync(u => u.Username == request.Username);
        if (usernameExists)
        {
            return Conflict(new { message = "Username is already taken." });
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            DisplayName = request.Username,
            IsActive = true,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var application = await _db.Applications.FirstOrDefaultAsync(a => a.Code == AuthServiceExtensions.RequiredAppAccessValue);
        if (application is not null)
        {
            _db.UserApplications.Add(new UserApplication { UserId = user.Id, ApplicationId = application.Id });
            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Registration successful." });
    }

    /// <summary>
    /// Authenticates a local username/password user and signs them in with a cookie session,
    /// including the app_access claim required by downstream authorization policies.
    /// </summary>
    [HttpPost("local-login")]
    public async Task<IActionResult> LocalLogin([FromBody] LocalLoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username ?? user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(AuthServiceExtensions.AppAccessClaimType, AuthServiceExtensions.RequiredAppAccessValue),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Ok(new { message = "Login successful." });
    }

    /// <summary>
    /// Triggers the OIDC challenge against AuthBridge. If the user already has an AuthBridge
    /// session, this results in silent (automatic) authentication.
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = "/")
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        return Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl ?? "/" },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Signs the user out of both the local cookie session and the AuthBridge OIDC session.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Returns the current user's authentication + application-access status for the Angular SPA.
    /// </summary>
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Ok(new { isAuthenticated = false });
        }

        var hasAccess = User.HasClaim(AuthServiceExtensions.AppAccessClaimType, AuthServiceExtensions.RequiredAppAccessValue);

        return Ok(new
        {
            isAuthenticated = true,
            isAuthorized = hasAccess,
            name = User.Identity?.Name,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
