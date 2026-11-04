using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Decision of a PlatformAdministrator on an appeal.
/// </summary>
public sealed record AppealDecision
{
    /// <summary>
    ///     Administrator who reviewed the appeal.
    /// </summary>
    public Guid AdministratorId { get; }

    /// <summary>
    ///     Outcome of the appeal.
    /// </summary>
    public AppealOutcome Outcome { get; }

    /// <summary>
    ///     Justification of the appeal decision.
    /// </summary>
    public Justification Justification { get; }

    /// <summary>
    ///     Moment of the appeal decision.
    /// </summary>
    public DateTimeOffset DecidedAt { get; }

    private AppealDecision(
        Guid administratorId,
        AppealOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        AdministratorId = administratorId;
        Outcome = outcome;
        Justification = justification;
        DecidedAt = decidedAt;
    }

    /// <summary>
    ///     Creates an appeal decision.
    /// </summary>
    public static AppealDecision Of(
        Guid administratorId,
        AppealOutcome outcome,
        Justification justification,
        DateTimeOffset decidedAt)
    {
        ArgumentNullException.ThrowIfNull(justification);

        if (administratorId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(administratorId));
        }

        if (!Enum.IsDefined(outcome))
        {
            throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "Unknown appeal outcome.");
        }

        return new AppealDecision(administratorId, outcome, justification, decidedAt);
    }
}