using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Snapshot of a master as a candidate for one request.
/// </summary>
public sealed record CandidateProfile
{
    /// <summary>
    ///     Identifier of the master.
    /// </summary>
    public Guid MasterId { get; }

    /// <summary>
    ///     Indicators of the master.
    /// </summary>
    public Performance Performance { get; }

    /// <summary>
    ///     Distance from the master to the request address.
    /// </summary>
    public double DistanceInKilometers { get; }

    /// <summary>
    ///     Radius of the master's service area.
    /// </summary>
    public double ServiceRadiusInKilometers { get; }

    /// <summary>
    ///     Whether the master is below the active requests limit and not cooling down.
    /// </summary>
    public bool CanTakeOn { get; }

    /// <summary>
    ///     Whether the master must not be offered requests in wave 1.
    /// </summary>
    public bool IsExcludedFromWaveOne { get; }

    /// <summary>
    ///     Whether the score of the master is lowered.
    /// </summary>
    public bool HasScorePenalty { get; }

    private CandidateProfile(
        Guid masterId,
        Performance performance,
        double distanceInKilometers,
        double serviceRadiusInKilometers,
        bool canTakeOn,
        bool isExcludedFromWaveOne,
        bool hasScorePenalty)
    {
        MasterId = masterId;
        Performance = performance;
        DistanceInKilometers = distanceInKilometers;
        ServiceRadiusInKilometers = serviceRadiusInKilometers;
        CanTakeOn = canTakeOn;
        IsExcludedFromWaveOne = isExcludedFromWaveOne;
        HasScorePenalty = hasScorePenalty;
    }

    internal static CandidateProfile Of(
        Guid masterId,
        Performance performance,
        double distanceInKilometers,
        double serviceRadiusInKilometers,
        bool canTakeOn,
        bool isExcludedFromWaveOne,
        bool hasScorePenalty)
    {
        if (!double.IsFinite(distanceInKilometers) || distanceInKilometers < 0)
        {
            throw new InvalidScoringDataException("Distance must be a non-negative number.");
        }

        if (!double.IsFinite(serviceRadiusInKilometers) || serviceRadiusInKilometers <= 0)
        {
            throw new InvalidScoringDataException("Service radius must be a positive number.");
        }

        return new CandidateProfile(
            masterId,
            performance,
            distanceInKilometers,
            serviceRadiusInKilometers,
            canTakeOn,
            isExcludedFromWaveOne,
            hasScorePenalty);
    }
}