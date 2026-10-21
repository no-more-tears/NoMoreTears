using Microsoft.Extensions.Logging;

namespace NoMoreTears.Shared.Postgres.Migrations.Diagnostics;

/// <summary>
///     Source-generated log messages of the migration manager.
/// </summary>
internal static partial class MigrationLogs
{
    [LoggerMessage(EventId = 2101, Level = LogLevel.Information,
        Message = "Migrations will not be applied for {DbContextName}. No pending migrations")]
    public static partial void NoPendingMigrations(this ILogger logger, string dbContextName);

    [LoggerMessage(EventId = 2102, Level = LogLevel.Information,
        Message = "Applying {Count} pending migrations for {DbContextName}: {Migrations}")]
    public static partial void ApplyingMigrations(this ILogger logger, int count, string dbContextName, string migrations);

    [LoggerMessage(EventId = 2103, Level = LogLevel.Information,
        Message = "Migrations applied successfully for {DbContextName} in {ElapsedMs}ms")]
    public static partial void MigrationsApplied(this ILogger logger, string dbContextName, long elapsedMs);

    [LoggerMessage(EventId = 2104, Level = LogLevel.Error,
        Message = "An error occurred while applying migrations for {DbContextName} after {ElapsedMs}ms")]
    public static partial void MigrationsFailed(this ILogger logger, Exception exception, string dbContextName, long elapsedMs);
}
