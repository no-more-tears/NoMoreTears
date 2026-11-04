using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Aggregates;

/// <summary>
///     Review with a rating left by the client of a completed request, once, within 14 days.
/// </summary>
public sealed class Review : AuditableEntity<Guid>
{
    /// <summary>
    ///     Reviewed request.
    /// </summary>
    public Guid RequestId { get; }

    /// <summary>
    ///     Client who left the review.
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    ///     Reviewed master.
    /// </summary>
    public Guid MasterId { get; }

    /// <summary>
    ///     Rating from 1 to 5.
    /// </summary>
    public Rating Rating { get; }

    /// <summary>
    ///     Optional text of the review.
    /// </summary>
    public ReviewComment? Comment { get; }

    /// <summary>
    ///     Status with respect to reputation.
    /// </summary>
    public ReviewStatus Status { get; private set; }

    /// <summary>
    ///     Weight in the reputation, lowered for a repeated client–master pair.
    /// </summary>
    public double Weight { get; private set; }

    /// <summary>
    ///     Incident that froze or excluded the review.
    /// </summary>
    public Guid? IncidentId { get; private set; }

    /// <summary>
    ///     Whether the review is verified and influences the reputation.
    /// </summary>
    public bool CountsTowardReputation => Status is ReviewStatus.Active && Weight > 0 && !IsDeleted;

    private Review(
        Guid id,
        Guid requestId,
        Guid clientId,
        Guid masterId,
        Rating rating,
        ReviewComment? comment)
    {
        Id = id;
        RequestId = requestId;
        ClientId = clientId;
        MasterId = masterId;
        Rating = rating;
        Comment = comment;
        Status = ReviewStatus.Active;
        Weight = 1;
    }

    /// <summary>
    ///     Leaves a review for a completed request.
    /// </summary>
    public static Review Create(
        RequestSnapshot request,
        Guid clientId,
        Rating rating,
        ReviewComment? comment,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(rating);

        if (request.State is not RequestState.Completed)
        {
            throw new ReviewNotAllowedException("A review can be left only for a completed request.");
        }

        if (request.ClientId != clientId)
        {
            throw new ReviewNotAllowedException("Only the client of the request can leave a review.");
        }

        if (createdAt > request.FinishedAt + TrustRules.ReviewWindow)
        {
            throw new ReviewNotAllowedException(
                $"A review can be left within {TrustRules.ReviewWindow.TotalDays} days after completion.");
        }

        var review = new Review(
            Guid.CreateVersion7(createdAt),
            request.RequestId,
            clientId,
            request.MasterId!.Value,
            rating,
            comment);
        review.MarkAsCreated(createdAt);

        return review;
    }

    /// <summary>
    ///     Freezes the review while an incident is reviewed.
    /// </summary>
    public void Freeze(
        Guid incidentId,
        DateTimeOffset frozenAt)
    {
        EnsureNotEmpty(incidentId);
        EnsureStatus(ReviewStatus.Active, "freeze");

        Status = ReviewStatus.Frozen;
        IncidentId = incidentId;
        MarkAsUpdated(frozenAt);
    }

    /// <summary>
    ///     Returns the review to the reputation when the incident did not confirm a violation.
    /// </summary>
    public void Unfreeze(
        Guid incidentId,
        DateTimeOffset unfrozenAt)
    {
        EnsureStatus(ReviewStatus.Frozen, "unfreeze");
        EnsureIncident(incidentId);

        Status = ReviewStatus.Active;
        IncidentId = null;
        MarkAsUpdated(unfrozenAt);
    }

    /// <summary>
    ///     Excludes the review from the reputation when the incident confirmed a manipulation.
    /// </summary>
    public void Exclude(
        Guid incidentId,
        DateTimeOffset excludedAt)
    {
        EnsureNotEmpty(incidentId);

        if (Status is ReviewStatus.Frozen)
        {
            EnsureIncident(incidentId);
        }
        else
        {
            EnsureStatus(ReviewStatus.Active, "exclude");
        }

        Status = ReviewStatus.Excluded;
        IncidentId = incidentId;
        MarkAsUpdated(excludedAt);
    }

    /// <summary>
    ///     Lowers the weight of the review of a repeated client–master pair.
    /// </summary>
    public void MarkAsRepeatedPair(DateTimeOffset markedAt)
    {
        Weight = Math.Min(Weight, TrustRules.RepeatedPairReviewWeight);
        MarkAsUpdated(markedAt);
    }

    private void EnsureStatus(ReviewStatus expected, string action)
    {
        if (Status != expected)
        {
            throw new InvalidReviewStatusException(Status, action);
        }
    }

    private void EnsureIncident(Guid incidentId)
    {
        if (IncidentId != incidentId)
        {
            throw new IncidentMismatchException(incidentId);
        }
    }

    private static void EnsureNotEmpty(Guid incidentId)
    {
        if (incidentId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(incidentId));
        }
    }
}