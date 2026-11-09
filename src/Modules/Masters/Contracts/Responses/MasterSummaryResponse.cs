namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Short master profile for review queues.
/// </summary>
public sealed record MasterSummaryResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Verification,
    string ServiceAreaStatus,
    DateTimeOffset? SubmittedAt,
    bool IsOnHold,
    DateTimeOffset? CreatedAt);
