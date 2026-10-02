using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Infrastructure.Auth;

namespace SmartHire.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
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
