using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;

namespace NoMoreTears.Modules.Masters.Domain.Entities;

/// <summary>
///     Catalog category chosen by a master together with the result of its qualification review.
/// </summary>
public sealed class MasterCategory
{
    /// <summary>
    ///     Identifier of the catalog category.
    /// </summary>
    public Guid CategoryId { get; }

    /// <summary>
    ///     Moment the category was added to the profile.
    /// </summary>
    public DateTimeOffset AddedAt { get; }

    /// <summary>
    ///     Review status of the qualification.
    /// </summary>
    public ReviewStatus Status { get; private set; }

    /// <summary>
    ///     Latest review decision.
    /// </summary>
    public ReviewDecision? Review { get; private set; }

    /// <summary>
    ///     Whether the qualification in this category is confirmed.
    /// </summary>
    public bool IsVerified => Status is ReviewStatus.Verified;

    private MasterCategory(Guid categoryId, DateTimeOffset addedAt)
    {
        CategoryId = categoryId;
        AddedAt = addedAt;
        Status = ReviewStatus.Pending;
    }

    /// <summary>
    ///     Creates a category awaiting review.
    ///     Only the <see cref="Aggregates.Master"/> aggregate adds categories.
    /// </summary>
    internal static MasterCategory Create(
        Guid categoryId,
        DateTimeOffset addedAt)
    {
        return new MasterCategory(categoryId, addedAt);
    }

    internal void Verify(ReviewDecision review)
    {
        EnsurePending("verify");

        Status = ReviewStatus.Verified;
        Review = review;
    }

    internal void Reject(ReviewDecision review)
    {
        EnsurePending("reject");

        Status = ReviewStatus.Rejected;
        Review = review;
    }

    internal void ReturnToReview()
    {
        Status = ReviewStatus.Pending;
        Review = null;
    }

    private void EnsurePending(string action)
    {
        if (Status is not ReviewStatus.Pending)
        {
            throw new InvalidReviewStatusException(Status, action);
        }
    }
}