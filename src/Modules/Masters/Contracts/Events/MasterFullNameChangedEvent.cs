using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master changed the first or last name (allowed only before verification or after a rejection).
/// </summary>
public sealed record MasterFullNameChangedEvent(
    Guid MasterId,
    string FirstName,
    string LastName,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
