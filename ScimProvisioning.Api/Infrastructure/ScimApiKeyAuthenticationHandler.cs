using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ScimProvisioning.Api.Infrastructure;

public class ScimApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "ScimApiKey";

    /// <summary>Shared secret issued exclusively to the SCIM provisioning client (AuthBridge).</summary>
    public string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Validates inbound SCIM requests using either an "Authorization: Bearer &lt;key&gt;" header
/// or an "X-Api-Key: &lt;key&gt;" header. This credential is dedicated to the SCIM provisioning
/// integration.
/// </summary>
public class ScimApiKeyAuthenticationHandler : AuthenticationHandler<ScimApiKeyAuthenticationOptions>
{
    public ScimApiKeyAuthenticationHandler(
        IOptionsMonitor<ScimApiKeyAuthenticationOptions> options,
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
                "SCIM provisioning is not configured (Scim:ApiKey is empty)."));
        }

        var presentedKey = ExtractPresentedKey();
        if (string.IsNullOrEmpty(presentedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing credential."));
        }

        if (!FixedTimeEquals(presentedKey, Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid credential."));
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "scim-provisioning-client") },
            ScimApiKeyAuthenticationOptions.SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string? ExtractPresentedKey()
    {
        if (Request.Headers.TryGetValue("X-Api-Key", out var apiKeyHeader) &&
            !string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            return apiKeyHeader.ToString();
        }

        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var value = authHeader.ToString();
            const string prefix = "Bearer ";
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return value[prefix.Length..].Trim();
            }
        }

        return null;
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
