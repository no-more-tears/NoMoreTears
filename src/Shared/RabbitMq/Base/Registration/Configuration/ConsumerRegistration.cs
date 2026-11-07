using MassTransit;

namespace NoMoreTears.Shared.RabbitMq.Registration.Configuration;

public sealed record ConsumerRegistration(
    Type ConsumerType,
    Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? ConfigureEndpoint);