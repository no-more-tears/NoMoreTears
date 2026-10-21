using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.CQS.Primitives.Queries;
using System.Text.Json;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for caching queries.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class CachingBehavior<TRequest, TResponse>(
    IDistributedCache distributedCache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachedQuery
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var cachedValue = await distributedCache.GetAsync(request.CacheKey, cancellationToken);

        if (cachedValue is not null)
        {
            try
            {
                var cached = JsonSerializer.Deserialize<TResponse>(cachedValue);

                if (cached is not null)
                {
                    logger.LogInformation(
                       "Response of request {RequestName} resolved from cache using key {CacheKey}",
                       requestName,
                       request.CacheKey);
                    return cached;
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(
                   ex,
                   "Cached value for request {RequestName} under key {CacheKey} could not be deserialized",
                   requestName,
                   request.CacheKey);
            }
        }

        var response = await next(cancellationToken);

        if (response is not null)
        {
            try
            {
                var serialized = JsonSerializer.SerializeToUtf8Bytes(response);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = request.Expiration ?? DefaultExpiration
                };

                await distributedCache.SetAsync(request.CacheKey, serialized, options, cancellationToken);

                logger.LogInformation(
                    "Response of request {RequestName} was cached under key {CacheKey}",
                    requestName,
                    request.CacheKey);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(
                    ex,
                    "Response of request {RequestName} could not be cached under key {CacheKey}",
                    requestName,
                    request.CacheKey);
            }
        }

        return response;
    }
}