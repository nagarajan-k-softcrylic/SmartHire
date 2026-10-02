namespace ScimProvisioning.Api.Infrastructure;

/// <summary>Configuration for pushing SCIM provisioning changes into SmartHire's own user store.</summary>
public class SmartHireSyncOptions
{
    public const string SectionName = "SmartHireSync";

    /// <summary>Base URL of the SmartHire application (e.g. https://localhost:7400).</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Shared secret for SmartHire's internal /internal/scim-sync/users endpoint.</summary>
    public string ApiKey { get; set; } = string.Empty;
}
