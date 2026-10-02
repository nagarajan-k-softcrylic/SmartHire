using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScimProvisioning.Api.Models;

/// <summary>
/// SCIM 2.0 PATCH request body (RFC 7644 §3.5.2).
/// </summary>
public class ScimPatchRequest
{
    public const string PatchOpSchema = "urn:ietf:params:scim:api:messages:2.0:PatchOp";

    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new() { PatchOpSchema };

    [JsonPropertyName("Operations")]
    public List<ScimPatchOperation> Operations { get; set; } = new();
}

public class ScimPatchOperation
{
    [JsonPropertyName("op")]
    public string Op { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }
}
