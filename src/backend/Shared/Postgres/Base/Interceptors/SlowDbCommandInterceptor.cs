using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace NoMoreTears.Shared.Postgres.Interceptors;

/// <summary>
///    An interceptor that logs a warning if a database command takes longer
///    than a specified threshold to execute.
/// </summary>
public sealed class SlowDbCommandInterceptor(
    ILogger<SlowDbCommandInterceptor> logger,
    TimeSpan threshold) : DbCommandInterceptor
{
    /// <inheritdoc/>
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command, eventData);
        return result;
    }

    /// <inheritdoc/>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken)
    {
        LogIfSlow(command, eventData);
        return new ValueTask<DbDataReader>(result);
    }

    /// <inheritdoc/>
    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        LogIfSlow(command, eventData);
        return result;
    }

    /// <inheritdoc/>
    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command, 
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken)
    {
        LogIfSlow(command, eventData);
        return new ValueTask<int>(result);
    }

    /// <inheritdoc/>
    public override object? ScalarExecuted(
        DbCommand command, 
        CommandExecutedEventData eventData,
        object? result)
    {
        LogIfSlow(command, eventData);
        return result;
    }

    /// <inheritdoc/>
    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken)
    {
        LogIfSlow(command, eventData);
        return new ValueTask<object?>(result);
    }

    /// <summary>
    ///     Logs a warning if the executed command took longer than the specified threshold.
    /// </summary>
    private void LogIfSlow(
        DbCommand command, 
        CommandExecutedEventData eventData)
    {
        if (eventData.Duration <= threshold)
        {
            return;
        }

        logger.LogWarning(
            "Slow database command detected. " +
            "Duration={DurationMs}ms > Threshold={ThresholdMs}ms, " +
            "Source={CommandSource}, CommandId={CommandId}",
            eventData.Duration.TotalMilliseconds,
            threshold.TotalMilliseconds,
            eventData.CommandSource,
            eventData.CommandId);

        logger.LogDebug(
            "Slow database command SQL: {CommandText}",
            command.CommandText);
    }
}