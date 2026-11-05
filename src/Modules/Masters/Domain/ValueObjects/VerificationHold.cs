namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Block of verification until a human check,
///     for example after a suspected duplicate account.
/// </summary>
public sealed record VerificationHold
{
    /// <summary>
    ///     Reason of the hold.
    /// </summary>
    public Reason Reason { get; }

    /// <summary>
    ///     Moment the hold was placed.
    /// </summary>
    public DateTimeOffset PlacedAt { get; }

    private VerificationHold(
        Reason reason,
        DateTimeOffset placedAt)
    {
        Reason = reason;
        PlacedAt = placedAt;
    }

    /// <summary>
    ///     Creates a verification hold.
    /// </summary>
    public static VerificationHold Of(
        Reason reason,
        DateTimeOffset placedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        return new VerificationHold(reason, placedAt);
    }
}