using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CompleteOverdueRequests;
using NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Configuration;

namespace NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Jobs;

/// <summary>
///     Completes requests marked as done when the client neither confirmed nor objected within 48 hours.
/// </summary>
public sealed class AutomaticCompletionJob(
    IServiceScopeFactory scopeFactory,
    IOptions<RequestJobsOptions> options,
    ILogger<AutomaticCompletionJob> logger) : DeadlineJob(scopeFactory, logger)
{
    protected override TimeSpan Interval => options.Value.CompletionInterval;

    protected override int BatchSize => options.Value.BatchSize;

    protected override Task<int> RunBatchAsync(ISender sender, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sender);

        return sender.Send(new CompleteOverdueRequestsCommand(BatchSize), cancellationToken);
    }
}