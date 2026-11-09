using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Migrations.Registration;

namespace NoMoreTears.Modules.Masters.HostSide.Migrations.Registration;

public static class HostRegistrar
{
    public const string LoggerCategory = "Masters.Migrations";

    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddMigrationHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMigrationManager<MastersDbContext, AssemblyReference>(
            configuration,
            ConnectionStringName,
            MastersDbContext.Schema);

        return services;
    }
}
