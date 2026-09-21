using Microsoft.Extensions.Logging;

namespace NoMoreTears.Shared.Postgres.Migrations.Abstractions;

/// <summary>
///     Database migration manager for applying migrations.
/// </summary>
public interface IDbMigrationManager
{
    /// <summary>
    ///     Apply pending migrations to the database.
    /// </summary>
    Task ApplyMigrationsAsync(
        ILogger logger, 
        CancellationToken cancellationToken);
}