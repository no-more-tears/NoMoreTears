using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Postgres.Migrations.Abstractions;
using NoMoreTears.Shared.Postgres.Migrations.Diagnostics;
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
                logger.NoPendingMigrations(dbContextName);
                return;
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.ApplyingMigrations(pendingMigrations.Count, dbContextName, string.Join(", ", pendingMigrations));
            }

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.MigrationsApplied(dbContextName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.MigrationsFailed(ex, dbContextName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
