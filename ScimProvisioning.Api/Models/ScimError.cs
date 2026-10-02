using System.Text.Json.Serialization;

namespace ScimProvisioning.Api.Models;

/// <summary>
/// SCIM 2.0 error response (RFC 7644 §3.12).
/// </summary>
public class ScimError
{
    public const string ErrorSchema = "urn:ietf:params:scim:api:messages:2.0:Error";

    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new() { ErrorSchema };

    [JsonPropertyName("status")]
    public string Status { get; set; } = "400";

    [JsonPropertyName("scimType")]
    public string? ScimType { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; } = string.Empty;

    public static ScimError Create(int statusCode, string detail, string? scimType = null) => new()
    {
        Status = statusCode.ToString(),
        ScimType = scimType,
        Detail = detail
    };
}
