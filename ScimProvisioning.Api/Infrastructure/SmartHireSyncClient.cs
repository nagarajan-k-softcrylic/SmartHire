using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ScimProvisioning.Api.Entities;

namespace ScimProvisioning.Api.Infrastructure;

/// <summary>
/// Pushes provisioned-user changes into SmartHire's internal sync endpoint. Failures are
/// logged, not thrown, so a SmartHire outage never fails the SCIM response back to AuthBridge.
/// </summary>
public class SmartHireSyncClient
{
    private readonly HttpClient _http;
    private readonly SmartHireSyncOptions _options;
    private readonly ILogger<SmartHireSyncClient> _logger;

    public SmartHireSyncClient(HttpClient http, IOptions<SmartHireSyncOptions> options, ILogger<SmartHireSyncClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SyncUserAsync(ProvisionedUser user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl) || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("SmartHireSync is not configured; skipping sync for provisioned user {UserId}.", user.Id);
            return;
        }

        try
        {
            var payload = new
            {
                externalId = user.Id.ToString(),
                userName = user.UserName,
                email = user.Email,
                givenName = user.GivenName,
                familyName = user.FamilyName,
                active = user.Active,
                groups = user.Groups.Select(g => g.Display).ToList()
            };

            using var request = new HttpRequestMessage(HttpMethod.Put, $"{_options.BaseUrl.TrimEnd('/')}/internal/scim-sync/users")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Add("X-Api-Key", _options.ApiKey);

            var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "SmartHire sync failed for provisioned user {UserId}: {StatusCode} {Body}",
                    user.Id, (int)response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SmartHire sync threw an exception for provisioned user {UserId}.", user.Id);
        }
    }
}
