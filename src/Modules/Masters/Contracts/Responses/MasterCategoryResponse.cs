namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Category of a master profile. <see cref="Status"/> is <c>Pending</c>, <c>Verified</c> or <c>Rejected</c>.
/// </summary>
public sealed record MasterCategoryResponse(
    Guid CategoryId,
    DateTimeOffset AddedAt,
    string Status,
    ReviewResponse? Review);
