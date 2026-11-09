using NoMoreTears.Modules.Masters.Domain.Enums;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Events;

/// <summary>
///     Builds integration events of the Masters module from the state returned by services.
/// </summary>
internal static class MastersEvents
{
    /// <summary>
    ///     Moment of the latest change of the profile.
    /// </summary>
    public static DateTimeOffset ChangedAt(MasterResponse master)
    {
        ArgumentNullException.ThrowIfNull(master);

        return master.LastUpdatedAt ?? master.CreatedAt
            ?? throw new InvalidOperationException($"Master {master.Id} has no change time.");
    }

    /// <summary>
    ///     Snapshot of the access of the master to the feed, for Dispatch and other consumers.
    /// </summary>
    public static MasterAccessUpdatedEvent AccessUpdated(MasterResponse master)
    {
        ArgumentNullException.ThrowIfNull(master);

        return new MasterAccessUpdatedEvent(
            master.Id,
            master.Verification,
            master.HasFeedAccess,
            master.IsAvailable,
            VerifiedCategoryIds(master),
            master.ServiceArea,
            master.ServiceAreaStatus == nameof(ReviewStatus.Verified),
            master.WorkSchedule,
            ActiveUntil(master, nameof(SanctionKind.Restriction)),
            ActiveUntil(master, nameof(SanctionKind.Suspension)),
            ChangedAt(master));
    }

    public static IReadOnlyList<Guid> VerifiedCategoryIds(MasterResponse master) =>
        [.. master.Categories
            .Where(category => category.Status == nameof(ReviewStatus.Verified))
            .Select(category => category.CategoryId)];

    public static IReadOnlyList<Guid> PendingCategoryIds(MasterResponse master) =>
        [.. master.Categories
            .Where(category => category.Status == nameof(ReviewStatus.Pending))
            .Select(category => category.CategoryId)];

    public static string ReviewReason(ReviewResponse? review) =>
        review?.Reason ?? string.Empty;

    public static ReviewResponse? CategoryReview(MasterResponse master, Guid categoryId) =>
        master.Categories.FirstOrDefault(category => category.CategoryId == categoryId)?.Review;

    /// <summary>
    ///     End of the active sanctions of a kind; <see cref="DateTimeOffset.MaxValue"/> means until lifted.
    /// </summary>
    private static DateTimeOffset? ActiveUntil(MasterResponse master, string kind)
    {
        var active = master.Sanctions
            .Where(sanction => sanction.IsActive && sanction.Kind == kind)
            .ToList();

        return active.Count == 0
            ? null
            : active.Max(sanction => sanction.ExpiresAt ?? DateTimeOffset.MaxValue);
    }
}
