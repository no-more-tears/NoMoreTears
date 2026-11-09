using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     Snapshot of everything that decides whether the master can receive requests. Published after every change
///     that may affect access (verification, review of categories or area, sanctions); consumers replace their copy.
/// </summary>
/// <remarks>
///     Restrictions and suspensions end by time without a new event, so <see cref="RestrictedUntil"/> and
///     <see cref="SuspendedUntil"/> must be compared with the current moment by the consumer.
/// </remarks>
public sealed record MasterAccessUpdatedEvent(
    Guid MasterId,
    string Verification,
    bool HasFeedAccess,
    bool IsAvailable,
    IReadOnlyList<Guid> VerifiedCategoryIds,
    ServiceAreaResponse ServiceArea,
    bool IsServiceAreaVerified,
    WorkScheduleResponse WorkSchedule,
    DateTimeOffset? RestrictedUntil,
    DateTimeOffset? SuspendedUntil,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
