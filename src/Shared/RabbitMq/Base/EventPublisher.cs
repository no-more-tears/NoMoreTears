using MassTransit;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq;

/// <summary>
///     Publishes events through the scoped publish endpoint of the bus.
/// </summary>
public sealed class EventPublisher(
    IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent =>
        await publishEndpoint.Publish(@event, cancellationToken);
}

/// <summary>
///     Default publisher of a module without a transactional outbox: publishes directly through the bus.
///     A module with a DbContext replaces it with an outbox publisher (<c>UsePostgresOutbox</c>).
/// </summary>
public sealed class EventPublisher<TModule>(
    IPublishEndpoint publishEndpoint) : IEventPublisher<TModule>
    where TModule : class
{
    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent =>
        await publishEndpoint.Publish(@event, cancellationToken);
}
