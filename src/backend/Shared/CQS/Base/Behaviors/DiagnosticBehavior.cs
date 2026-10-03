using MediatR;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using System.Diagnostics;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for diagnostics and distributed tracing of incoming requests.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class DiagnosticBehavior<TRequest, TResponse>(
    IEnumerable<IDiagnosticHandler<TRequest, TResponse>> handlers)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    /// <summary>
    ///     Name of the <see cref="ActivitySource"/> used for all CQS pipeline spans.
    /// </summary>
    public const string ActivitySourceName = "HauteCouture.CQS";

    private static readonly ActivitySource Source = new(ActivitySourceName);

    private readonly IReadOnlyList<IDiagnosticHandler<TRequest, TResponse>> _handlers =
        handlers as IReadOnlyList<IDiagnosticHandler<TRequest, TResponse>> ?? handlers.ToList();

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var activityName = typeof(TRequest).Name;

        using var activity = Source.StartActivity(activityName, ActivityKind.Internal);

        foreach (var handler in _handlers)
        {
            handler.Before(request, activity);
        }

        try
        {
            var response = await next(cancellationToken);
            activity?.SetStatus(ActivityStatusCode.Ok);

            foreach (var handler in _handlers)
            {
                handler.After(activity, request, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);

            throw;
        }
    }
}