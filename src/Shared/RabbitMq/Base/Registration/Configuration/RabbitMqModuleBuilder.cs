using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.RabbitMq.Abstractions;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Shared.RabbitMq.Registration.Configuration;

/// <summary>
///     Messaging part of one module on the shared bus of the host.
/// </summary>
public abstract class RabbitMqModuleBuilder
{
    private readonly List<Action<IBusRegistrationConfigurator>> _busConfigurations = [];
    private readonly List<Action<IRegistrationContext, IReceiveEndpointConfigurator>> _endpointConfigurations = [];
    private readonly HashSet<string> _endpointNames = new(StringComparer.Ordinal);

    private protected RabbitMqModuleBuilder(
        IServiceCollection services,
        string endpointPrefix)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointPrefix);

        Services = services;
        EndpointPrefix = endpointPrefix;
    }

    /// <summary>
    ///     Services of the host, for registrations that belong to the messaging of the module.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    ///     Prefix of the queue names of the module, so that consumers of different modules never share a queue.
    /// </summary>
    public string EndpointPrefix { get; }

    internal IReadOnlyCollection<Action<IBusRegistrationConfigurator>> BusConfigurations => _busConfigurations;

    internal bool OwnsEndpoint(string endpointName) => _endpointNames.Contains(endpointName);

    internal void ConfigureEndpoint(IRegistrationContext context, IReceiveEndpointConfigurator endpoint)
    {
        foreach (var configure in _endpointConfigurations)
        {
            configure(context, endpoint);
        }
    }

    private protected void AddEndpoint<TConsumer>()
        where TConsumer : class, IConsumer
    {
        var endpointName = $"{EndpointPrefix}-{KebabCaseEndpointNameFormatter.Instance.Consumer<TConsumer>()}";

        _endpointNames.Add(endpointName);
        _busConfigurations.Add(configurator => configurator
            .AddConsumer<TConsumer>()
            .Endpoint(endpoint => endpoint.Name = endpointName));
    }

    private protected void AddBusConfiguration(Action<IBusRegistrationConfigurator> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _busConfigurations.Add(configure);
    }

    private protected void AddEndpointConfiguration(Action<IRegistrationContext, IReceiveEndpointConfigurator> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _endpointConfigurations.Add(configure);
    }
}

/// <summary>
///     Messaging part of the module <typeparamref name="TModule"/> on the shared bus of the host.
/// </summary>
/// <typeparam name="TModule">Marker of the module, used by <see cref="IEventPublisher{TModule}"/>.</typeparam>
public sealed class RabbitMqModuleBuilder<TModule> : RabbitMqModuleBuilder
    where TModule : class
{
    internal RabbitMqModuleBuilder(
        IServiceCollection services,
        string endpointPrefix)
        : base(services, endpointPrefix)
    {
    }

    /// <summary>
    ///     Registers a consumer of <typeparamref name="TEvent"/> in a queue named
    ///     <c>{prefix}-{consumer name in kebab case}</c> with the default retry and concurrency settings.
    /// </summary>
    public RabbitMqModuleBuilder<TModule> AddConsumer<TEvent, TConsumer>()
        where TConsumer : class, IEventConsumer<TEvent>
        where TEvent : class, IEvent
    {
        AddEndpoint<TConsumer>();
        return this;
    }

    /// <summary>
    ///     Adds a customization of the bus registration, for example a transactional outbox.
    /// </summary>
    public RabbitMqModuleBuilder<TModule> ConfigureBus(Action<IBusRegistrationConfigurator> configure)
    {
        AddBusConfiguration(configure);
        return this;
    }

    /// <summary>
    ///     Adds a configuration applied only to the receive endpoints of the module's consumers,
    ///     after the retry policy (for example the consumer outbox and inbox).
    /// </summary>
    public RabbitMqModuleBuilder<TModule> ConfigureEndpoints(Action<IRegistrationContext, IReceiveEndpointConfigurator> configure)
    {
        AddEndpointConfiguration(configure);
        return this;
    }
}