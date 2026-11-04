using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Step of the sanction ladder from 1 to 4.
/// </summary>
public sealed record SanctionStep
{
    /// <summary>
    ///     Lowest step: a warning.
    /// </summary>
    public const int First = 1;

    /// <summary>
    ///     Highest step: deverification of a master or an account block of a client.
    /// </summary>
    public const int Last = 4;

    /// <summary>
    ///     Number of the step.
    /// </summary>
    public int Value { get; }

    private SanctionStep(int value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a ladder step.
    /// </summary>
    public static SanctionStep Of(int value)
    {
        if (value is < First or > Last)
        {
            throw new SanctionNotAllowedException($"Sanction step must be from {First} to {Last}.");
        }

        return new SanctionStep(value);
    }

    /// <summary>
    ///     Returns the step that follows the latest sanction of a subject, or the first step if there is none.
    /// </summary>
    public static SanctionStep After(SanctionStep? latest)
    {
        return latest is null ? new SanctionStep(First) : new SanctionStep(Math.Min(Last, latest.Value + 1));
    }

    /// <summary>
    ///     Returns the sanction of this step for a participant with the given role.
    /// </summary>
    public SanctionKind KindFor(PartyRole role)
    {
        return (role, Value) switch
        {
            (_, 1) => SanctionKind.Warning,
            (PartyRole.Master, 2) => SanctionKind.Restriction,
            (PartyRole.Master, 3) => SanctionKind.Suspension,
            (PartyRole.Master, _) => SanctionKind.Deverification,
            (PartyRole.Client, 2) => SanctionKind.PublicationRestriction,
            (PartyRole.Client, 3) => SanctionKind.PublicationBlock,
            _ => SanctionKind.AccountBlock
        };
    }

    /// <summary>
    ///     Returns how long the sanction of this step lasts.
    /// </summary>
    public TimeSpan? Duration => Value switch
    {
        2 => TrustRules.RestrictionDuration,
        3 => TrustRules.SuspensionDuration,
        _ => null
    };
}