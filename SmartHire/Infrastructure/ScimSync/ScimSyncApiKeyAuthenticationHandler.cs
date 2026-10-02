using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SmartHire.Infrastructure.ScimSync;

public class ScimSyncApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "ScimSyncApiKey";

    /// <summary>Shared secret issued exclusively to ScimProvisioning.Api for the internal sync call.</summary>
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Validates the internal service-to-service call from ScimProvisioning.Api using an
/// "X-Api-Key" header. This credential is dedicated to that integration and is distinct
/// from AuthBridge's own SCIM credential and from end-user login tokens.
/// </summary>
public class ScimSyncApiKeyAuthenticationHandler : AuthenticationHandler<ScimSyncApiKeyAuthenticationOptions>
{
    public ScimSyncApiKeyAuthenticationHandler(
        IOptionsMonitor<ScimSyncApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrEmpty(Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail(
                "SCIM sync is not configured (ScimSync:ApiKey is empty)."));
        }

        if (!Request.Headers.TryGetValue("X-Api-Key", out var presented) || string.IsNullOrWhiteSpace(presented))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing credential."));
        }

        if (!FixedTimeEquals(presented.ToString(), Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid credential."));
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "scim-sync-client") },
            ScimSyncApiKeyAuthenticationOptions.SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var bytesA = Encoding.UTF8.GetBytes(a);
        var bytesB = Encoding.UTF8.GetBytes(b);
        if (bytesA.Length != bytesB.Length)
        {
            return CryptographicOperations.FixedTimeEquals(bytesA, bytesA) && false;
        }

        return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }
}
