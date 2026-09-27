using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for detecting and logging slow requests
///     that exceed a configured execution time threshold.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int WarningThresholdMilliseconds = 800;

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > WarningThresholdMilliseconds)
        {
            logger.LogWarning(
                "Detected slow request {RequestName} taking {ElapsedMilliseconds} ms, exceeding the {ThresholdMilliseconds} ms threshold",
                 requestName,
                 stopwatch.ElapsedMilliseconds,
                 WarningThresholdMilliseconds);
        }

        return response;
    }
}