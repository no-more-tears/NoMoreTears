namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Short request for lists.
/// </summary>
public sealed record RequestSummaryResponse(
    Guid Id,
    Guid ClientId,
    Guid CategoryId,
    string Status,
    int PublicationCount,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? AcceptanceDeadline,
    Guid? MasterId,
    DateTimeOffset? ExpectedArrivalAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? LastUpdatedAt);