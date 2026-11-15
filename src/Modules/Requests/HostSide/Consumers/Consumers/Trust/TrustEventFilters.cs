using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Trust.Contracts.Constants;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Selects the Trust decisions that affect requests and translates them into restrictions (anti-corruption layer).
/// </summary>
internal static class TrustEventFilters
{
    public const string PublicationLimit = nameof(RestrictionKind.PublicationLimit);

    private const string PublicationBlock = nameof(RestrictionKind.PublicationBlock);

    public static bool IsAboutClient(string subjectRole) =>
        string.Equals(subjectRole, PartyRoles.Client, StringComparison.Ordinal);

    public static bool IsComplaint(string kind) =>
        string.Equals(kind, IncidentKinds.Complaint, StringComparison.Ordinal);

    /// <summary>
    ///     Restriction of a client sanction (section 4.3): step 2 limits publication to one request,
    ///     step 3 and a blocked account forbid it; other sanctions do not affect publication.
    /// </summary>
    public static string? RestrictionOf(string sanctionKind) => sanctionKind switch
    {
        SanctionKinds.PublicationRestriction => PublicationLimit,
        SanctionKinds.PublicationBlock or SanctionKinds.AccountBlock => PublicationBlock,
        _ => null
    };

    public static bool LimitsPublication(IReadOnlyCollection<string> effects) =>
        effects.Contains(MeasureEffects.PublicationLimit, StringComparer.Ordinal);
}