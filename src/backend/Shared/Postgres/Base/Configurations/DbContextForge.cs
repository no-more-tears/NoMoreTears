using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Postgres.Extensions;
using NoMoreTears.Shared.Postgres.Interceptors;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace NoMoreTears.Shared.Postgres.Configurations;

/// <summary>
///    An abstract base class for configuring a DbContext with PostgreSQL settings.
/// </summary>
/// <remarks>
///     See <c>Configurations\README.md</c> for EF Core performance tuning recommendations.
/// </remarks>
/// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
public abstract class DbContextForge<TDbContext>(
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
    : IDbContextConfigurator<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Used to create a DbContext configuration based on the provided template.
    /// </summary>
    protected abstract DbContextBlueprint Draft(DbContextBlueprint blueprint);

    /// <summary>
    ///     Provides a collection of interceptors for tooling purposes.
    /// </summary>
    /// <remarks>
    ///     By default, a <see cref="SlowDbCommandInterceptor"/> is also added to log slow queries.
    /// </remarks>
    protected virtual IEnumerable<IInterceptor> Tooling() => [];

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
        Configure(optionsBuilder, _ => { });
    }

    /// <inheritdoc />
    public void Configure(
        DbContextOptionsBuilder<TDbContext> optionsBuilder, 
        Action<NpgsqlDbContextOptionsBuilder> optionsAction)
    {
        var blueprint = Draft(new DbContextBlueprint());
        var connectionString = configuration.GetConnectionString(blueprint.ConnectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{blueprint.ConnectionString}' could not be found.");
        }

        // It is better to use the configuration that is defined by default; it is selected with optimality.
        optionsBuilder
            .UseLoggerFactory(loggerFactory)
            .UseNpgsql(connectionString, builder =>
            {
                // Configure performance for Npgsql-specific options.
                builder
                    .CommandTimeout(blueprint.CommandTimeout)
                    .MaxBatchSize(blueprint.MaxBatchSize)
                    .EnableRetryOnFailure(
                        maxRetryCount: blueprint.MaxRetryCount,
                        maxRetryDelay: blueprint.MaxRetryDelay,
                        errorCodesToAdd: null);

                // Additional custom options customization provided by the caller.
                optionsAction(builder);
            })
            .UseNamingStyle(blueprint.NamingStyle)
            .AddInterceptors(
            [
                ..Tooling(),
                new SlowDbCommandInterceptor(
                    loggerFactory.CreateLogger<SlowDbCommandInterceptor>(),
                    blueprint.SlowQueryThreshold)
            ]);

        if (blueprint.SensitiveDataLogging)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        if (blueprint.DetailedErrorsEnabled)
        {
            optionsBuilder.EnableDetailedErrors();
        }
    }
}