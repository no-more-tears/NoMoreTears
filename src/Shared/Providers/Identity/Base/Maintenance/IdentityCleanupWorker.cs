using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Maintenance;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

namespace NoMoreTears.Shared.Providers.Identity.Maintenance;

/// <summary>
///     Hosted service that runs <see cref="IIdentityMaintenanceService"/> on a fixed interval.
/// </summary>
internal sealed class IdentityCleanupWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<MaintenanceOptions> options,
    ILogger<IdentityCleanupWorker> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunOnceAsync(stoppingToken);
            }
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
            await using var scope = scopeFactory.CreateAsyncScope();
            var maintenance = scope.ServiceProvider.GetRequiredService<IIdentityMaintenanceService>();
            await maintenance.PurgeExpiredAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // A failed run must not stop the host; the next tick retries.
            logger.LogError(exception, "Identity maintenance failed.");
        }
    }
}