namespace NoMoreTears.Shared.HostSide.WebApi.Modules.TrafficControl;

/// <summary>
///     Configuration options for resolving the client address behind reverse proxies.
/// </summary>
public sealed class TrafficControlOptions
{
    public const string SectionName = "TrafficControl";

    /// <summary>
    ///     Addresses of reverse proxies whose X-Forwarded-For header is trusted.
    ///     Loopback is always trusted; requests from any other address use the connection address.
    /// </summary>
    public string[] KnownProxies { get; init; } = [];
}