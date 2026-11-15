using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Migrations.Registration;

namespace NoMoreTears.Modules.Requests.HostSide.Migrations.Registration;

public static class HostRegistrar
{
    public const string LoggerCategory = "Requests.Migrations";

    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddMigrationHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMigrationManager<RequestsDbContext, AssemblyReference>(
            configuration,
            ConnectionStringName,
            RequestsDbContext.Schema);

        return services;
    }
}