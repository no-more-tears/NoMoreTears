namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Status of an incident in arbitration.
/// </summary>
public enum IncidentStatus
{
    Open,
    InReview,
    Decided,
    UnderAppeal,
    Closed
}