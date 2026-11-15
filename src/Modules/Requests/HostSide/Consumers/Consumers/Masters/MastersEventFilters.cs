using NoMoreTears.Modules.Masters.Contracts.Events;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Translates the access of a master published by Masters into the terms of Requests (anti-corruption layer).
/// </summary>
internal static class MastersEventFilters
{
    private const string Verified = nameof(Verified);

    /// <summary>
    ///     Categories the master may accept: the verified categories of a verified master with a verified service area
    ///     (FR-MST-03); none otherwise.
    /// </summary>
    public static IReadOnlyList<Guid> AcceptableCategoryIds(MasterAccessUpdatedEvent message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var isVerified = string.Equals(message.Verification, Verified, StringComparison.Ordinal)
            && message.IsServiceAreaVerified;

        return isVerified ? message.VerifiedCategoryIds : [];
    }
}