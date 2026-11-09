namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Full master profile, as seen by the master and by reviewers.
///     <see cref="Verification"/> is <c>NotSubmitted</c>, <c>Pending</c>, <c>Verified</c>, <c>Rejected</c> or <c>Revoked</c>.
/// </summary>
public sealed record MasterResponse(
    Guid Id,
    string FirstName,
    string LastName,
    AvatarResponse? Avatar,
    string Verification,
    ReviewResponse? VerificationReview,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? VerifiedAt,
    DateTimeOffset? RevokedAt,
    DateTimeOffset? ReverificationAllowedFrom,
    VerificationHoldResponse? VerificationHold,
    ServiceAreaResponse ServiceArea,
    string ServiceAreaStatus,
    ReviewResponse? ServiceAreaReview,
    WorkScheduleResponse WorkSchedule,
    bool IsAvailable,
    bool HasFeedAccess,
    bool IsRestricted,
    bool IsSuspended,
    IReadOnlyList<MasterCategoryResponse> Categories,
    IReadOnlyList<DocumentResponse> Documents,
    IReadOnlyList<SanctionResponse> Sanctions,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? LastUpdatedAt);
