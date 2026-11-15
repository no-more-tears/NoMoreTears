using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExpireOverdueRequests;
using NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Configuration;

namespace NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Jobs;

/// <summary>
///     Expires published requests whose acceptance deadline has passed.
/// </summary>
public sealed class RequestExpirationJob(
    IServiceScopeFactory scopeFactory,
    IOptions<RequestJobsOptions> options,
    ILogger<RequestExpirationJob> logger) : DeadlineJob(scopeFactory, logger)
{
    protected override TimeSpan Interval => options.Value.ExpirationInterval;

    protected override int BatchSize => options.Value.BatchSize;

    protected override Task<int> RunBatchAsync(ISender sender, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sender);

        return sender.Send(new ExpireOverdueRequestsCommand(BatchSize), cancellationToken);
    }
}