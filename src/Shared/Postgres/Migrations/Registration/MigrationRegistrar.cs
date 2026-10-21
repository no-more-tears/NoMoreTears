using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Postgres.Migrations.Abstractions;

namespace NoMoreTears.Shared.Postgres.Migrations.Registration;

/// <summary>
///     Registrar for managing db migrations.
/// </summary>
public static class MigrationRegistrar
{
    /// <summary>
    ///     Adds the migration manager with registered DbContext and assembly for migrations host.
    /// </summary>
    public static IServiceCollection AddMigrationManager<TDbContext, TAssembly>(
        this IServiceCollection services, 
        IConfiguration configuration,
        string connectionString)
        where TDbContext : DbContext
    {
        services
            .AddDbContext<TDbContext>(optionsBuilder =>
            {
                optionsBuilder
                    .UseNpgsql(GetConnectionString(configuration, connectionString), builder =>
                    {
                        // Configuration for migrations in case of a failure when connecting to db.
                        builder
                            .MigrationsAssembly(typeof(TAssembly).Assembly.FullName)
                            .CommandTimeout(30)
                            .EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorCodesToAdd: null);
                    });
            })
            .AddSingleton<IDbMigrationManager, DbMigrationManager<TDbContext>>();

        return services;
    }

    /// <summary>
    ///     Gets the connection string from the configuration.
    /// </summary>
    private static string GetConnectionString(
        IConfiguration configuration,
        string connectionStringName)
    {
        var connectionString  = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{connectionStringName}' could not be found.");
        }

        return connectionString;
    }
}