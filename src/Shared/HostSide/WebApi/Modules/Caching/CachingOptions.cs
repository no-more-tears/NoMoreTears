namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Caching;

/// <summary>
///     Configuration options for the Redis cache connection.
/// </summary>
public sealed class CachingOptions
{
    public const string SectionName = "RedisCache";

    /// <summary>
    ///     Redis server connection string.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     Password used to authenticate with the Redis server.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///     Prefix applied to all cache keys to distinguish entries across applications or environments.
    /// </summary>
    public required string InstanceName { get; init; }
}