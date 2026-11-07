namespace NoMoreTears.Shared.RabbitMq.Contracts;

public interface IEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}