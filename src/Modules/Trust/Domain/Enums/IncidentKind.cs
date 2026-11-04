namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Origin of an incident: a complaint of a participant or a fraud signal.
/// </summary>
public enum IncidentKind
{
    Complaint,
    RatingBurst,
    SelfBoosting,
    RepeatedNoShows,
    MasterCancellationSeries,
    GeoAnomaly
}