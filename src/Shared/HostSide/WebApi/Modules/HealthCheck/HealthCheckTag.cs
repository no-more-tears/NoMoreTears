namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck;

/// <summary>
///     Defines standard tags used to categorize health check probes.
/// </summary>
public static class HealthCheckTag
{
    /// <summary>
    ///     Indicates the service process is running and able to handle requests.
    /// </summary>
    public const string Liveness = "live";

    /// <summary>
    ///     Indicates all infrastructure dependencies are reachable.
    /// </summary>
    public const string Readiness = "ready";
}