using MassTransit;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq.Registration.Configuration;

public sealed class RabbitMqConsumerBuilder
{
    private readonly List<ConsumerRegistration> _consumers = [];

    public IReadOnlyCollection<ConsumerRegistration> Consumers => _consumers.AsReadOnly();

    private RabbitMqConsumerBuilder() { }

    public static RabbitMqConsumerBuilder CreateBuilder() => new();

    public RabbitMqConsumerBuilder AddConsumer<TEvent, TConsumer>(
        Action<IRabbitMqReceiveEndpointConfigurator>? configure = null)
        where TConsumer : IEventConsumer<TEvent>
        where TEvent : class, IEvent
    {
        _consumers.Add(new ConsumerRegistration(
            typeof(TConsumer),
            (context, busCfg) =>
            {
                busCfg.ReceiveEndpoint(endpointCfg =>
                {
                    endpointCfg.UseMessageRetry(retryCfg =>
                    {
                        retryCfg.Exponential(
                            retryLimit: 5,
                            minInterval: TimeSpan.FromSeconds(1),
                            maxInterval: TimeSpan.FromSeconds(30),
                            intervalDelta: TimeSpan.FromSeconds(5));
                    });

                    endpointCfg.ConcurrentMessageLimit = 8;
                    endpointCfg.PrefetchCount = 16;

                    endpointCfg.ConfigureConsumers(context);
                    configure?.Invoke(endpointCfg);
                });
            }));

        return this;
    }
}