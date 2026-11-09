using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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
    /// <param name="schema">Schema of the migrations history table; <c>null</c> keeps the provider default.</param>
    public static IServiceCollection AddMigrationManager<TDbContext, TAssembly>(
        this IServiceCollection services, 
        IConfiguration configuration,
        string connectionString,
        string? schema = null)
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

                        // The history table lives in the schema of the module tables.
                        if (!string.IsNullOrWhiteSpace(schema))
                        {
                            builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
                        }
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