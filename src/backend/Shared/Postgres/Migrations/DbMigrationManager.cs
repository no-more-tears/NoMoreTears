using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Postgres.Migrations.Abstractions;
using System.Diagnostics;

namespace NoMoreTears.Shared.Postgres.Migrations;

/// <summary>
///     Default database migration manager for applying migrations.
/// </summary>
public sealed class DbMigrationManager<TDbContext>(
    IServiceProvider serviceProvider)
    : IDbMigrationManager
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider = serviceProvider
        ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public async Task ApplyMigrationsAsync(
        ILogger logger, 
        CancellationToken cancellationToken)
    {
        var dbContextName = typeof(TDbContext).Name;
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<TDbContext>()
            ?? throw new InvalidOperationException(
                $"Database connection for {dbContextName} is not available. " +
                $"Unable to resolve an instance of {dbContextName}");

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (pendingMigrations.Count == 0)
            {
                logger.LogInformation(
                    "Migrations will not be applied for {DbContextName}. No pending migrations", dbContextName);
                return;
            }

            logger.LogInformation(
                "Applying {Count} pending migrations for {DbContextName}: {Migrations}",
                pendingMigrations.Count, dbContextName, string.Join(", ", pendingMigrations));

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation(
                 "Migrations applied successfully for {DbContextName} in {ElapsedMs}ms",
                 dbContextName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while applying migrations for {DbContextName} after {ElapsedMs}ms",
                dbContextName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}