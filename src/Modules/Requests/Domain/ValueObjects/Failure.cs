using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Outcome of a failed execution.
/// </summary>
public sealed record Failure
{
    /// <summary>
    ///     Kind of failure.
    /// </summary>
    public FailureKind Kind { get; }

    /// <summary>
    ///     Master responsible for the failed execution.
    /// </summary>
    public Guid MasterId { get; }

    /// <summary>
    ///     Reason of the refusal.
    /// </summary>
    public Reason? Reason { get; }

    private Failure(
        FailureKind kind,
        Guid masterId,
        Reason? reason)
    {
        Kind = kind;
        MasterId = masterId;
        Reason = reason;
    }

    /// <summary>
    ///     Creates a failure.
    /// </summary>
    public static Failure Of(
        FailureKind kind,
        Guid masterId,
        Reason? reason)
    {
        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        if (kind is FailureKind.MasterRefusal && reason is null)
        {
            throw new InvalidFailureException("A refusal by the master requires a reason.");
        }

        return new Failure(kind, masterId, reason);
    }
}