namespace NoMoreTears.Shared.RabbitMq.Contracts;

/// <summary>
///     Base of the integration events exchanged between modules.
/// </summary>
/// <param name="OccurredAt">Moment the fact described by the event happened.</param>
public abstract record IntegrationEvent(DateTimeOffset OccurredAt) : IEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.CreateVersion7();
}