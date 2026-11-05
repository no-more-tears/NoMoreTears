using NoMoreTears.Modules.Dispatch.Domain.Exceptions;
using NoMoreTears.Modules.Dispatch.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Aggregates;

/// <summary>
///     Immutable version of the dispatch parameters with the moment it takes effect.
///     A change of parameters creates a new version, so all versions form the audit trail.
/// </summary>
public sealed class DispatchPolicy : AuditableEntity<Guid>
{
    /// <summary>
    ///     Weights of the scoring factors.
    /// </summary>
    public ScoringWeights Weights { get; }

    /// <summary>
    ///     Bonus added to the score of wave 1 candidates of a republished request.
    /// </summary>
    public double RepublicationBonus { get; }

    /// <summary>
    ///     Share by which the score of a restricted master is lowered.
    /// </summary>
    public double RestrictionPenalty { get; }

    /// <summary>
    ///     Waves ordered by number; the last one is final.
    /// </summary>
    public IReadOnlyList<WaveSettings> Waves { get; }

    /// <summary>
    ///     Quota of slots for newcomers.
    /// </summary>
    public NewcomerQuota NewcomerQuota { get; }

    /// <summary>
    ///     Limits of masters.
    /// </summary>
    public MasterLimits Limits { get; }

    /// <summary>
    ///     Moment the version takes effect.
    /// </summary>
    public DateTimeOffset EffectiveFrom { get; }

    private DispatchPolicy(
        Guid id,
        ScoringWeights weights,
        double republicationBonus,
        double restrictionPenalty,
        IReadOnlyList<WaveSettings> waves,
        NewcomerQuota newcomerQuota,
        MasterLimits limits,
        DateTimeOffset effectiveFrom)
    {
        Id = id;
        Weights = weights;
        RepublicationBonus = republicationBonus;
        RestrictionPenalty = restrictionPenalty;
        Waves = waves;
        NewcomerQuota = newcomerQuota;
        Limits = limits;
        EffectiveFrom = effectiveFrom;
    }

    /// <summary>
    ///     Creates a version of the dispatch parameters.
    /// </summary>
    public static DispatchPolicy Create(
        ScoringWeights weights,
        double republicationBonus,
        double restrictionPenalty,
        IReadOnlyCollection<WaveSettings> waves,
        NewcomerQuota newcomerQuota,
        MasterLimits limits,
        DateTimeOffset effectiveFrom,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(weights);
        ArgumentNullException.ThrowIfNull(waves);
        ArgumentNullException.ThrowIfNull(newcomerQuota);
        ArgumentNullException.ThrowIfNull(limits);

        if (!double.IsFinite(republicationBonus) || republicationBonus < 0 || republicationBonus > 1)
        {
            throw new InvalidPolicyException("Republication bonus must be from 0 to 1.");
        }

        if (!double.IsFinite(restrictionPenalty) || restrictionPenalty < 0 || restrictionPenalty >= 1)
        {
            throw new InvalidPolicyException("Restriction penalty must be from 0 to less than 1.");
        }

        var orderedWaves = waves.OrderBy(wave => wave.Number).ToArray();
        EnsureWavePlan(orderedWaves);

        var policy = new DispatchPolicy(
            Guid.CreateVersion7(createdAt),
            weights,
            republicationBonus,
            restrictionPenalty,
            orderedWaves,
            newcomerQuota,
            limits,
            effectiveFrom);
        policy.MarkAsCreated(createdAt);

        return policy;
    }

    /// <summary>
    ///     Returns the settings of the wave with the given number.
    /// </summary>
    public WaveSettings WaveSettingsFor(int number) => Waves[number - 1];

    private static void EnsureWavePlan(WaveSettings[] waves)
    {
        if (waves.Length == 0)
        {
            throw new InvalidPolicyException("The wave plan must contain at least one wave.");
        }

        for (var index = 0; index < waves.Length; index++)
        {
            var wave = waves[index];

            if (wave.Number != index + 1)
            {
                throw new InvalidPolicyException("Waves must be numbered consecutively from 1.");
            }

            if (wave.IsFinal != (index == waves.Length - 1))
            {
                throw new InvalidPolicyException("Only the last wave must be final.");
            }

            if (index > 0 && wave.TopShare <= waves[index - 1].TopShare)
            {
                throw new InvalidPolicyException("Each wave must offer the request to a larger share of candidates.");
            }
        }
    }
}