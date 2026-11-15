namespace NoMoreTears.Modules.Trust.Contracts.Constants;

/// <summary>
///     Kinds of incidents in integration events.
/// </summary>
public static class IncidentKinds
{
    public const string Complaint = nameof(Complaint);

    public const string RatingBurst = nameof(RatingBurst);

    public const string SelfBoosting = nameof(SelfBoosting);

    public const string RepeatedNoShows = nameof(RepeatedNoShows);

    public const string MasterCancellationSeries = nameof(MasterCancellationSeries);

    public const string GeoAnomaly = nameof(GeoAnomaly);
}