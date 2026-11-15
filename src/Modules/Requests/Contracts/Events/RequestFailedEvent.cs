using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Execution failed (FR-REQ-07): <see cref="Kind"/> is <c>NoShow</c> (reported by the client) or <c>MasterRefusal</c> (with a reason). Trust counts no-shows.
/// </summary>
public sealed record RequestFailedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid MasterId,
    string Kind,
    string? Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);