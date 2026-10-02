using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace SmartHire.Infrastructure.Auth;

/// <summary>
/// Shared AuthBridge OIDC authentication configuration so future downstream applications
/// (e.g. Report Generator) can reuse the exact same pattern.
/// </summary>
public static class AuthServiceExtensions
{
    public const string AppAccessClaimType = "app_access";
    public const string RequiredAppAccessValue = "RESUME_AI";
    public const string RequireAppAccessPolicy = "RequireResumeAiAccess";

    public static IServiceCollection ConfigureAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Oidc:Authority"] ?? "https://localhost:7199";
        var clientId = configuration["Oidc:ClientId"] ?? "RESUME_AI";

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.ClientId = clientId;
                options.RequireHttpsMetadata = false;
                options.ResponseType = "code";
                options.UsePkce = true;
                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.SaveTokens = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("application_access");
            });

        services.AddAuthorization(options =>
        {
            // Claim-based authorization: user must present app_access = RESUME_AI.
            options.AddPolicy(RequireAppAccessPolicy, policy =>
                policy.RequireClaim(AppAccessClaimType, RequiredAppAccessValue));
        });

        return services;
    }
}
