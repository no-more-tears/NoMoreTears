using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

/// <summary>
///     Provides contextual infrastructure data available during module mounting.
/// </summary>
public sealed class WebModuleContext
{
    /// <summary>
    ///     Application's DI service collection.
    /// </summary>
    public required IServiceCollection Services { get; init; }

    /// <summary>
    ///     Application configuration.
    /// </summary>
    public required IConfiguration Configuration { get; init; }

    /// <summary>
    ///     Web hosting environment.
    /// </summary>
    public required IWebHostEnvironment Environment { get; init; }

    /// <summary>
    ///     Host builder for advanced scenarios where module needs to configure the host.
    /// </summary>
    public required IHostBuilder Host { get; init; }
}