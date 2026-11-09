using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     A temporary measure ended: it was lifted by a human or expired. Confirmed measures are replaced by sanctions.
/// </summary>
public sealed record TemporaryMeasureLiftedEvent(
    Guid MeasureId,
    Guid SubjectId,
    string SubjectRole,
    string Rule,
    IReadOnlyList<string> Effects,
    DateTimeOffset LiftedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
