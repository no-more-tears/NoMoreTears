using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq.Abstractions;

/// <summary>
///     Publishes integration events directly through the bus of the host.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent;
}

/// <summary>
///     Publishes integration events on behalf of a module.
/// </summary>
/// <remarks>
///     All modules of a host share one bus; <typeparamref name="TModule"/> selects the transactional outbox of the
///     module (its DbContext), so the event is committed together with the data of that module. Inside a consumer
///     the event goes through the consumer outbox of the module.
/// </remarks>
/// <typeparam name="TModule">Marker of the module.</typeparam>
public interface IEventPublisher<TModule>
    where TModule : class
{
    Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent;
}