using MassTransit;
using MassTransit.Observables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;

namespace NoMoreTears.Shared.RabbitMq.Registration;

public static class RabbitMqRegistrar
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<RabbitMqConsumerBuilder>? configure = null)
    {
        var options = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()
            ?? throw new InvalidOperationException(
                $"Section '{RabbitMqOptions.SectionName}' is not configured.");

        var builder = RabbitMqConsumerBuilder.CreateBuilder();
        configure?.Invoke(builder);

        services
            .AddMassTransit(cfg =>
            {
                foreach (var consumer in builder.Consumers)
                {
                    cfg.AddConsumer(consumer.ConsumerType);
                }

                cfg.SetKebabCaseEndpointNameFormatter();
                cfg.AddTelemetryListener();

                cfg.UsingRabbitMq((context, rabbitMqCfg) =>
                {
                    rabbitMqCfg.Host(options.HostName, (ushort)options.Port, options.VirtualHost, hostCfg =>
                    {
                        hostCfg.Username(options.Username);
                        hostCfg.Password(options.Password);
                    });

                    rabbitMqCfg.ConnectSendObserver(new SendObservable());
                    rabbitMqCfg.ConnectReceiveObserver(new ReceiveObservable());

                    rabbitMqCfg.ConfigureEndpoints(context);
                });
            })
            .AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}