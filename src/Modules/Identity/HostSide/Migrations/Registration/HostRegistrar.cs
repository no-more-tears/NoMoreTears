using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Identity.Migrations.Registration;

namespace NoMoreTears.Modules.Identity.HostSide.Migrations.Registration;

public static class HostRegistrar
{
    public const string LoggerCategory = "Identity.Migrations";

    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddMigrationHost<TAssembly>(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityMigrationManager<TAssembly>(
            configuration,
            ConnectionStringName);

        return services;
    }
}