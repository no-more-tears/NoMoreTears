using NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

namespace NoMoreTears.Modules.Dispatch.Domain.Entities;

/// <summary>
///     Master selected for a distribution.
/// </summary>
public sealed class Candidate
{
    /// <summary>
    ///     Identifier of the master.
    /// </summary>
    public Guid MasterId { get; }

    /// <summary>
    ///     Wave in which the request is offered to the master.
    /// </summary>
    public int Wave { get; }

    /// <summary>
    ///     Whether the master took a newcomer slot.
    /// </summary>
    public bool IsNewcomer { get; }

    /// <summary>
    ///     Score of the master for the request and its explanation.
    /// </summary>
    public Score Score { get; private set; }

    /// <summary>
    ///     Moment the request appeared in the master's feed.
    /// </summary>
    public DateTimeOffset? OfferedAt { get; private set; }

    /// <summary>
    ///     Whether the request is in the master's feed.
    /// </summary>
    public bool IsOffered => OfferedAt is not null;

    private Candidate(
        Guid masterId,
        int wave,
        bool isNewcomer,
        Score score)
    {
        MasterId = masterId;
        Wave = wave;
        IsNewcomer = isNewcomer;
        Score = score;
    }

    /// <summary>
    ///     Creates a candidate. Only the <see cref="Aggregates.Distribution"/> aggregate plans candidates.
    /// </summary>
    internal static Candidate Create(
        Guid masterId,
        int wave,
        bool isNewcomer,
        Score score)
    {
        return new Candidate(masterId, wave, isNewcomer, score);
    }

    internal void AddBonus(double bonus)
    {
        Score = Score.WithBonus(bonus);
    }

    internal void Offer(DateTimeOffset offeredAt)
    {
        OfferedAt ??= offeredAt;
    }
}