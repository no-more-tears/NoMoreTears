using MassTransit;
using MassTransit.Observables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;

namespace NoMoreTears.Shared.RabbitMq.Registration;

/// <summary>
///     Registrar for RabbitMQ messaging through MassTransit.
/// </summary>
/// <remarks>
///     A host has exactly one bus. Modules describe their messaging with <see cref="AddRabbitMqModule{TModule}"/>
///     (consumers, transactional outbox of their DbContext), then the host calls <see cref="AddRabbitMq"/> once,
///     after every module was registered.
/// </remarks>
public static class RabbitMqRegistrar
{
    /// <summary>
    ///     Number of redeliveries of a failed message before it is moved to the error queue.
    /// </summary>
    public const int RetryLimit = 5;

    private const int ConcurrentMessageLimit = 8;
    private const int PrefetchCount = 16;

    /// <summary>
    ///     Describes the messaging of a module; it takes effect when the host calls <see cref="AddRabbitMq"/>.
    ///     Registers <see cref="IEventPublisher{TModule}"/> that publishes directly, unless the module configures an outbox.
    /// </summary>
    /// <param name="endpointPrefix">Prefix of the queue names of the module, for example <c>masters</c>.</param>
    public static IServiceCollection AddRabbitMqModule<TModule>(
        this IServiceCollection services,
        string endpointPrefix,
        Action<RabbitMqModuleBuilder<TModule>>? configure = null)
        where TModule : class
    {
        ArgumentNullException.ThrowIfNull(services);

        var module = new RabbitMqModuleBuilder<TModule>(services, endpointPrefix);

        services.AddSingleton<RabbitMqModuleBuilder>(module);
        services.TryAddScoped<IEventPublisher<TModule>, EventPublisher<TModule>>();

        configure?.Invoke(module);

        return services;
    }

    /// <summary>
    ///     Registers the bus of the host with the consumers and customizations of every module described before,
    ///     and <see cref="IEventPublisher"/>.
    /// </summary>
    /// <param name="configuration">Root configuration (<see cref="RabbitMqOptions.SectionName"/>).</param>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()
            ?? throw new InvalidOperationException(
                $"Section '{RabbitMqOptions.SectionName}' is not configured.");

        var modules = services
            .Where(descriptor => descriptor.ServiceType == typeof(RabbitMqModuleBuilder))
            .Select(descriptor => (RabbitMqModuleBuilder)descriptor.ImplementationInstance!)
            .ToList();

        services
            .AddMassTransit(cfg => ConfigureBus(cfg, options, modules))
            .AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }

    private static void ConfigureBus(
        IBusRegistrationConfigurator cfg,
        RabbitMqOptions options,
        IReadOnlyCollection<RabbitMqModuleBuilder> modules)
    {
        cfg.SetKebabCaseEndpointNameFormatter();

        foreach (var configureBus in modules.SelectMany(module => module.BusConfigurations))
        {
            configureBus(cfg);
        }

        // Retries wrap the inbox/outbox of the module that owns the endpoint.
        cfg.AddConfigureEndpointsCallback((context, endpointName, endpoint) =>
        {
            endpoint.UseMessageRetry(retry => retry.Exponential(
                retryLimit: RetryLimit,
                minInterval: TimeSpan.FromSeconds(1),
                maxInterval: TimeSpan.FromSeconds(30),
                intervalDelta: TimeSpan.FromSeconds(5)));

            endpoint.ConcurrentMessageLimit = ConcurrentMessageLimit;

            if (endpoint is IRabbitMqReceiveEndpointConfigurator rabbitMqEndpoint)
            {
                rabbitMqEndpoint.PrefetchCount = PrefetchCount;
            }

            foreach (var module in modules.Where(module => module.OwnsEndpoint(endpointName)))
            {
                module.ConfigureEndpoint(context, endpoint);
            }
        });

        cfg.AddTelemetryListener();

        cfg.UsingRabbitMq((context, rabbitMqCfg) =>
        {
            rabbitMqCfg.Host(options.HostName, (ushort)options.Port, options.VirtualHost, hostCfg =>
            {
                hostCfg.Username(options.Username);
                hostCfg.Password(options.Password);
            });

            // Only concrete events get exchanges; the shared base types would only add useless bindings.
            rabbitMqCfg.Publish<IEvent>(publish => publish.Exclude = true);
            rabbitMqCfg.Publish<IntegrationEvent>(publish => publish.Exclude = true);

            rabbitMqCfg.ConnectSendObserver(new SendObservable());
            rabbitMqCfg.ConnectReceiveObserver(new ReceiveObservable());

            rabbitMqCfg.ConfigureEndpoints(context);
        });
    }
}