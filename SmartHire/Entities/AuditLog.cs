namespace SmartHire.Entities;

/// <summary>
/// Audit trail entry for logins, searches, screening requests and indexing events.
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string EventType { get; set; } = string.Empty; // Login, Search, Screening, Indexing
    public string? Description { get; set; }
    public string? Metadata { get; set; } // JSON payload
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
