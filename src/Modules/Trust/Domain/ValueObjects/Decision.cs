using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Decision of a PlatformSupport arbiter with the mandatory justification.
/// </summary>
public sealed record Decision
{
    /// <summary>
    ///     Arbiter who made the decision.
    /// </summary>
    public Guid ArbiterId { get; }

    /// <summary>
    ///     Outcome of the decision.
    /// </summary>
    public DecisionOutcome Outcome { get; }

    /// <summary>
    ///     Justification of the decision.
    /// </summary>
    public Justification Justification { get; }

    /// <summary>
    ///     Moment of the decision.
    /// </summary>
    public DateTimeOffset DecidedAt { get; }

    private Decision(
        Guid arbiterId,
        DecisionOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        ArbiterId = arbiterId;
        Outcome = outcome;
        Justification = justification;
        DecidedAt = decidedAt;
    }

    /// <summary>
    ///     Creates a decision.
    /// </summary>
    public static Decision Of(
        Guid arbiterId,
        DecisionOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        ArgumentNullException.ThrowIfNull(justification);

        if (arbiterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(arbiterId));
        }

        if (!Enum.IsDefined(outcome))
        {
            throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "Unknown decision outcome.");
        }

        return new Decision(arbiterId, outcome, justification, decidedAt);
    }
}