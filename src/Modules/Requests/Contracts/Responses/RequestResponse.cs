namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Full request. <see cref="Address"/> is <c>null</c> for a master until the request is accepted by them.
/// </summary>
public sealed record RequestResponse(
    Guid Id,
    Guid ClientId,
    Guid CategoryId,
    AddressResponse? Address,
    string Description,
    string Status,
    int PublicationCount,
    bool IsRepublished,
    DateTimeOffset? PublishedAt,
    DateTimeOffset? AcceptanceDeadline,
    AssignmentResponse? Assignment,
    DateTimeOffset? ConfirmationDeadline,
    DateTimeOffset? CompletedAt,
    FailureResponse? Failure,
    DateTimeOffset? CancelledAt,
    string? CancellationReason,
    Guid? IncidentId,
    string? StatusBeforeDispute,
    IReadOnlyList<MediaFileResponse> MediaFiles,
    StatusChangeResponse LastStatusChange,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? LastUpdatedAt);