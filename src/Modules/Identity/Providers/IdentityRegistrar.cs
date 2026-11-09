using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Modules.Identity.Applications.Services;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Configuration;
using NoMoreTears.Modules.Identity.HostSide.Endpoints;
using NoMoreTears.Modules.Identity.Applications.Services.Messaging;
using NoMoreTears.Shared.CQS.Registration;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;
using NoMoreTears.Shared.Postgres.Extensions;
using NoMoreTears.Shared.Postgres.Registration;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.RabbitMq.Registration;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;

namespace NoMoreTears.Modules.Identity.Providers;

/// <summary>
///     Registrar for identity services.
/// </summary>
public static class IdentityRegistrar
{
    /// <summary>
    ///     Registers Applications (services + handlers).
    /// </summary>
    public static IServiceCollection AddApplications(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddServices(configuration)
            .AddHandlers();

        return services;
    }

    /// <summary>
    ///     Registers identity Applications services.
    /// </summary>
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<IdentityLinkOptions>()
            .Bind(configuration.GetSection(IdentityLinkOptions.SectionName));

        services.TryAddSingleton(TimeProvider.System);

        services
            .AddScoped<IIdentityService, IdentityService>()
            .AddScoped<IUserService, UserService>();

        return services;
    }

    /// <summary>
    ///     Registers identity Applications handlers.
    /// </summary>
    /// <remarks>
    ///     Payload logging is not enabled: identity requests and responses carry passwords and tokens.
    ///     Services log the outcome of each operation instead.
    /// </remarks>
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services
            .AddCqs(options => options
                .FromAssembly(Applications.Handlers.AssemblyReference.Assembly)
                .UseAuthorization()
                .UseValidation()
                .UsePerformanceTracking()
                .UseTransactions());

        // Identity services save their own changes (failed sign-in counters, revocations must survive a failed
        // command), so commands are not wrapped in a transaction: only the outbox messages are saved afterwards.
        services.AddTransactionalScope<IdentityDbContext>(
            Applications.Handlers.AssemblyReference.Assembly,
            TransactionMode.SaveChanges);

        return services;
    }

    /// <summary>
    ///     Registers identity endpoints.
    /// </summary>
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services
            .AddEndpoints(HostSide.Endpoints.AssemblyReference.Assembly);

        return services;
    }

    /// <summary>
    ///     Describes the messaging of the module on the shared bus of the host: the transactional outbox in the
    ///     identity schema. Identity only publishes events (emails to deliver); it has no consumers.
    /// </summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        return services.AddRabbitMqModule<IIdentityOutbox>("identity", module => module
            .UsePostgresOutbox<IIdentityOutbox, IdentityDbContext>());
    }
}
