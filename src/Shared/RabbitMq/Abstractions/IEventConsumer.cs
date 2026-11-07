using MassTransit;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq.Abstractions;

public interface IEventConsumer<TEvent>
    : IConsumer<TEvent>
    where TEvent : class, IEvent;