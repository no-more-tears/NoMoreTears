namespace NoMoreTears.Shared.HostSide.SignalR.Registration.Configuration;

/// <summary>
///     Options of the real-time host: hub transport tuning and the Redis backplane
///     that lets several instances deliver messages to connections of each other.
/// </summary>
public sealed class SignalROptions
{
    public const string SectionName = "SignalR";

    /// <summary>
    ///     Redis connection string of the backplane, for example <c>localhost:6379</c>.
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Password of the Redis server; empty when the server has no password.
    /// </summary>
    public string? RedisPassword { get; set; }

    /// <summary>
    ///     Prefix of the Redis channels, so that several environments can share one Redis server.
    /// </summary>
    public string ChannelPrefix { get; set; } = "nomoretears";

    /// <summary>
    ///     Interval of the keep-alive pings sent by the server.
    /// </summary>
    public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    ///     Time without any message from a client after which the connection is closed;
    ///     should be about twice the keep-alive interval of the client.
    /// </summary>
    public TimeSpan ClientTimeoutInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Maximum size of a single incoming hub message in bytes.
    /// </summary>
    public long MaximumReceiveMessageSize { get; set; } = 32 * 1024;

    /// <summary>
    ///     Path prefix of the hubs; requests under it may pass the access token in the <c>access_token</c>
    ///     query parameter, because browsers cannot set headers on WebSocket requests.
    /// </summary>
    public string HubsPathPrefix { get; set; } = "/hubs";
}