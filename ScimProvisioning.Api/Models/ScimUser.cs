using System.Text.Json.Serialization;

namespace ScimProvisioning.Api.Models;

/// <summary>
/// SCIM 2.0 User resource (RFC 7643 core User schema), trimmed to the attributes
/// this service actually persists.
/// </summary>
public class ScimUser
{
    public const string UserSchema = "urn:ietf:params:scim:schemas:core:2.0:User";

    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new() { UserSchema };

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public ScimName? Name { get; set; }

    [JsonPropertyName("emails")]
    public List<ScimEmail> Emails { get; set; } = new();

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("groups")]
    public List<ScimGroup> Groups { get; set; } = new();
}

public class ScimName
{
    [JsonPropertyName("givenName")]
    public string? GivenName { get; set; }

    [JsonPropertyName("familyName")]
    public string? FamilyName { get; set; }
}

public class ScimEmail
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}

public class ScimGroup
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("display")]
    public string? Display { get; set; }
}
