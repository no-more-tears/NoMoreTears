using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for logging data to improve analysis.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    IHttpContextAccessor contextAccessor) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    /// <summary>
    ///     Default fallback value used when HTTP context data is unavailable.
    /// </summary>
    private const string Undefined = "Undefined";

    /// <summary>
    ///     JSON serialization options used for logging payloads.
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    { 
        WriteIndented = false 
    };

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var httpRequest = contextAccessor.HttpContext?.Request;
        var method = httpRequest?.Method ?? Undefined;
        var path = httpRequest?.Path.Value ?? Undefined;

        var requestJson = TrySerialize(request, logger);
        logger.LogInformation(
           "Request {RequestName} started handling {Method} {Path} with payload {Request}",
           requestName,
           method,
           path,
           requestJson);

        var response = await next(cancellationToken);

        var responseJson = TrySerialize(response, logger);
        logger.LogInformation(
           "Request {RequestName} finished handling {Method} {Path} with response {Response}",
           requestName,
           method,
           path,
           responseJson);

        return response;
    }

    /// <summary>
    ///     Attempts to serialize the specified payload to JSON for logging purposes.
    /// </summary>
    private static string TrySerialize<TPayload>(
        TPayload payload,
        ILogger logger)
    {
        if (payload is null)
        {
            return string.Empty;
        }

        try
        {
            // TODO: Once the Common layer has been created,
            // check the attribute to see if the data is sensitive.
            return JsonSerializer.Serialize(payload, SerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Payload of type {PayloadType} could not be serialized for logging", 
                typeof(TPayload).Name);
            return string.Empty;
        }
    }
}