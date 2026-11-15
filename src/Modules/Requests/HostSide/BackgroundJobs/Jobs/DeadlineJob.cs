using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Jobs;

/// <summary>
///     Runs one deadline operation on a fixed interval.
/// </summary>
public abstract class DeadlineJob(
    IServiceScopeFactory scopeFactory,
    ILogger logger) : BackgroundService
{
    /// <summary>
    ///     Interval between runs.
    /// </summary>
    protected abstract TimeSpan Interval { get; }

    /// <summary>
    ///     Requests processed in one batch.
    /// </summary>
    protected abstract int BatchSize { get; }

    /// <summary>
    ///     Sends the command of one batch and returns the number of processed requests.
    /// </summary>
    protected abstract Task<int> RunBatchAsync(ISender sender, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            do
            {
                await RunOnceAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown.
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            int processed;

            do
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                processed = await RunBatchAsync(sender, cancellationToken);
            }
            while (processed >= BatchSize && !cancellationToken.IsCancellationRequested);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Background job {Job} failed.", GetType().Name);
        }
    }
}