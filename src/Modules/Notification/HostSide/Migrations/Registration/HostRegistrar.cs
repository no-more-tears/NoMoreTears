using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Migrations.Registration;

namespace NoMoreTears.Modules.Notification.HostSide.Migrations.Registration;

public static class HostRegistrar
{
    public const string LoggerCategory = "Notification.Migrations";

    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddMigrationHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMigrationManager<NotificationDbContext, AssemblyReference>(
            configuration,
            ConnectionStringName,
            NotificationDbContext.Schema);

        return services;
    }
}