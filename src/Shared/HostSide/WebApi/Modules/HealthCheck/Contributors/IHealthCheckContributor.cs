using Microsoft.Extensions.DependencyInjection;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors;

/// <summary>
///     Represents an opt-in contributor that registers a named health check probe
///     into the <see cref="IHealthChecksBuilder"/>.
/// </summary>
public interface IHealthCheckContributor
{
    /// <summary>
    ///     Registers the health check probe into the builder.
    /// </summary>
    void Register(IHealthChecksBuilder builder);
}