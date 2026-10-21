using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NoMoreTears.Shared.Postgres.Diagnostics;

/// <summary>
///     Source-generated log messages of the Postgres module.
/// </summary>
internal static partial class PostgresLogs
{
    [LoggerMessage(EventId = 2001, Level = LogLevel.Warning,
        Message = "Slow database command detected. Duration={DurationMs}ms > Threshold={ThresholdMs}ms, Source={CommandSource}, CommandId={CommandId}")]
    public static partial void SlowCommandDetected(
        this ILogger logger,
        double durationMs,
        double thresholdMs,
        CommandSource commandSource,
        Guid commandId);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Debug,
        Message = "Slow database command SQL: {CommandText}")]
    public static partial void SlowCommandSql(this ILogger logger, string commandText);
}
