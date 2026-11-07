using MassTransit;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq;

public sealed class EventPublisher(
    IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(
        TEvent @event, 
        CancellationToken cancellationToken)
        where TEvent : class, IEvent =>
        await publishEndpoint.Publish(@event, cancellationToken);
}