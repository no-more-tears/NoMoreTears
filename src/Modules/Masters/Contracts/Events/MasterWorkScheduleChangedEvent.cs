using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The weekly work schedule changed; Dispatch uses it to select candidates.
/// </summary>
public sealed record MasterWorkScheduleChangedEvent(
    Guid MasterId,
    WorkScheduleResponse WorkSchedule,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
