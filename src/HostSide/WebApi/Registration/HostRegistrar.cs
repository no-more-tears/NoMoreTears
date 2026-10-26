using NoMoreTears.Modules.Identity.Providers;
using NoMoreTears.Modules.Notification.Providers;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Caching;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;
using NoMoreTears.Shared.HostSide.WebApi.Registration;
using NoMoreTears.Shared.Providers.Hosting;
using System.Reflection;

namespace NoMoreTears.HostSide.WebApi.Registration;

/// <summary>
///     Provides extension methods for registering the WebApi service's.
/// </summary>
public static class HostRegistrar
{
    private const string PostgresConnectionStringName = "Postgres";

    extension (IServiceCollection services)
    {
        /// <summary>
        ///     Adds WebApi host modules.
        /// </summary>
        public IServiceCollection AddWebApiHostModules(IConfiguration configuration) =>
            services.AddModules(configuration,
                new NotificationModule(),
                new IdentityModule());

        /// <summary>
        ///     Registers all services required by the WebApi host.
        /// </summary>
        public IServiceCollection AddWebApiHost(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var (redisConnectionString, redisPassword) = GetRedisOptions(configuration);

            services
                .AddEndpointsApiExplorer()
                .AddWebModules(configuration, environment, host, options => options
                    .FromAssemblies(assembly)
                    .UseCaching()
                    .UseLogging()
                    .UseExceptionHandling()
                    .UseTrafficControl()
                    .UseUserSessions()
                    .UseSwagger()
                    .UseHealthChecks(health => health
                        .UsePostgres(new PostgresHealthCheckOptions
                        {
                            ConnectionString = GetPostgresConnectionString(configuration)
                        })
                        .UseRedis(new RedisHealthCheckOptions
                        {
                            ConnectionString = redisConnectionString,
                            Password = redisPassword
                        })
                    ));

            return services;
        }
    }

    private static string GetPostgresConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString(PostgresConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{PostgresConnectionStringName}' was not found.");
    }

    private static (string ConnectionString, string Password) GetRedisOptions(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(CachingOptions.SectionName)
            .Get<CachingOptions>()
                ?? throw new InvalidOperationException(
                $"Missing required configuration section '{CachingOptions.SectionName}'.");

        return (options.ConnectionString, options.Password);
    }
}