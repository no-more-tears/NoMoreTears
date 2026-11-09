using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.Postgres.Messaging;

/// <summary>
///     Publishes events of the module <typeparamref name="TModule"/> through the transactional outbox
///     of <typeparamref name="TDbContext"/>.
/// </summary>
/// <remarks>
///     Outside a consumer the event is added to the <c>OutboxMessage</c> table of the context and is delivered after
///     the context is saved and committed (the unit of work of the module does it). Inside a consumer the event goes
///     through the consume context, i.e. the consumer outbox. The outbox provider is resolved per context, so several
///     modules with their own contexts can publish through the one bus of a host.
/// </remarks>
public sealed class OutboxEventPublisher<TModule, TDbContext>(
    EntityFrameworkScopedBusContextProvider<IBus, TDbContext> outbox) : IEventPublisher<TModule>
    where TModule : class
    where TDbContext : DbContext
{
    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : class, IEvent =>
        await outbox.Context.PublishEndpoint.Publish(@event, cancellationToken);
}