namespace SmartHire.Features;

/// <summary>
/// Marker class used for MediatR/FluentValidation assembly scanning.
/// Vertical-slice features (e.g. Screening, Synchronization, Candidates) will be added here as
/// MediatR Commands/Queries + Handlers + Validators during implementation.
/// </summary>
public sealed class FeaturesAssemblyMarker
{
}
