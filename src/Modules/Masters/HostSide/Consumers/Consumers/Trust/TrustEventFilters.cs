using NoMoreTears.Modules.Trust.Contracts.Constants;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Selects the Trust decisions that affect master profiles (anti-corruption layer).
/// </summary>
internal static class TrustEventFilters
{
    private static readonly HashSet<string> MasterSanctionKinds = new(StringComparer.Ordinal)
    {
        SanctionKinds.Warning,
        SanctionKinds.Restriction,
        SanctionKinds.Suspension,
        SanctionKinds.Deverification
    };

    public static bool IsAboutMaster(string subjectRole) =>
        string.Equals(subjectRole, PartyRoles.Master, StringComparison.Ordinal);

    public static bool IsMasterSanction(string kind) => MasterSanctionKinds.Contains(kind);

    public static bool SuspendsFeed(IReadOnlyCollection<string> effects) =>
        effects.Contains(MeasureEffects.FeedSuspension, StringComparer.Ordinal);

    public static bool HoldsVerification(IReadOnlyCollection<string> effects) =>
        effects.Contains(MeasureEffects.VerificationHold, StringComparer.Ordinal);
}
