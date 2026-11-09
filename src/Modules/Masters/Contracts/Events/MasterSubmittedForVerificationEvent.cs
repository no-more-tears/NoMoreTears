using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master submitted the profile for verification; reviewers (PlatformSupport) get a new item in the queue.
/// </summary>
public sealed record MasterSubmittedForVerificationEvent(
    Guid MasterId,
    IReadOnlyList<Guid> CategoryIdsUnderReview,
    bool IsServiceAreaUnderReview,
    bool IsReverification,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
