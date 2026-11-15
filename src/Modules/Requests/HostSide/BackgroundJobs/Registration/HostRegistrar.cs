using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Configuration;
using NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Jobs;
using NoMoreTears.Modules.Requests.Providers;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;

namespace NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Registration;

/// <summary>
///     Registers the Requests background jobs host. Events of the jobs are published through the outbox
///     of the module, so the host has a bus but no consumers.
/// </summary>
public static class HostRegistrar
{
    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddBackgroundJobsHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RequestJobsOptions>()
            .Bind(configuration.GetSection(RequestJobsOptions.SectionName))
            .Validate(options => options.ExpirationInterval > TimeSpan.Zero
                && options.CompletionInterval > TimeSpan.Zero
                && options.BatchSize > 0, "Intervals and the batch size of the jobs must be positive.")
            .ValidateOnStart();

        services
            // Jobs act on behalf of the system, not of a user.
            .AddUserSessions(_ => new CurrentUserSession())
            .AddRequestsDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddRequestsApplications(configuration)
            .AddRequestsMessaging()
            .AddRabbitMq(configuration);

        services
            .AddHostedService<RequestExpirationJob>()
            .AddHostedService<AutomaticCompletionJob>();

        return services;
    }
}