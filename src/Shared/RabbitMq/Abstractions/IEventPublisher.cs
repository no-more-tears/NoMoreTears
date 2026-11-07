using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent;
}