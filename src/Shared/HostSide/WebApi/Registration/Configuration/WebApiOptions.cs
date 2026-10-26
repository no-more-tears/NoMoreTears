using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Configuration;
using System.Reflection;

namespace NoMoreTears.Shared.HostSide.WebApi.Registration.Configuration;

/// <summary>
///     Configuration options for registering web modules.
/// </summary>
public sealed class WebApiOptions
{
    /// <summary>
    ///     Indicates whether the logging web module is enabled.
    /// </summary>
    internal bool LoggingEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the caching web module is enabled.
    /// </summary>
    internal bool CachingEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the throttling web module is enabled.
    /// </summary>
    internal bool TrafficControlEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the global exception handling web module is enabled.
    /// </summary>
    internal bool ExceptionHandlingEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the health check web module is enabled.
    /// </summary>
    internal bool HealthCheckEnabled { get; private set; }

    /// <summary>
    ///     Health check module configuration. Set when <see cref="UseHealthChecks"/> is called.
    /// </summary>
    internal HealthCheckOptions? HealthCheckOptions { get; private set; }

    /// <summary>
    ///     Indicates whether the authorization (current user session) web module is enabled.
    /// </summary>
    internal bool AuthorizationEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the Swagger web module is enabled.
    /// </summary>
    internal bool SwaggerEnabled { get; private set; }

    /// <summary>
    ///     Assemblies to scan for <see cref="IWebModule"/> implementations.
    /// </summary>
    internal Assembly[] Assemblies { get; private set; } = [];

    /// <summary>
    ///     Explicitly provided module instances to register alongside discovered ones.
    /// </summary>
    internal IWebModule[] ExplicitModules { get; private set; } = [];

    /// <summary>
    ///     Application configuration passed to each module via <see cref="WebModuleContext"/>.
    /// </summary>
    internal IConfiguration Configuration { get; set; } = null!;

    /// <summary>
    ///     Web hosting environment passed to each module via <see cref="WebModuleContext"/>.
    /// </summary>
    internal IWebHostEnvironment Environment { get; set; } = null!;

    /// <summary>
    ///     Application host builder passed to each module via <see cref="WebModuleContext"/>.
    /// </summary>
    internal IHostBuilder Host { get; set; } = null!;

    /// <summary>
    ///     Scans the specified assemblies for <see cref="IWebModule"/> implementations.
    /// </summary>
    public WebApiOptions FromAssemblies(params Assembly[] assemblies)
    {
        Assemblies = assemblies;
        return this;
    }

    /// <summary>
    ///     Registers additional module instances alongside discovered ones.
    /// </summary>
    public WebApiOptions WithModules(params IWebModule[] modules)
    {
        ExplicitModules = modules;
        return this;
    }

    /// <summary>
    ///     Enables the logging web module (<see cref="Modules.Logging.LoggingWebModule"/>).
    /// </summary>
    public WebApiOptions UseLogging()
    {
        LoggingEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the caching web module (<see cref="Modules.Caching.CachingWebModule"/>).
    /// </summary>
    public WebApiOptions UseCaching()
    {
        CachingEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the throttling and rate limiting web module (<see cref="Modules.TrafficControl.ThrottlingAndRateLimitingWebModule"/>).
    /// </summary>
    public WebApiOptions UseTrafficControl()
    {
        TrafficControlEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the global exception handling web module.
    /// </summary>
    public WebApiOptions UseExceptionHandling()
    {
        ExceptionHandlingEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the health check web module with the specified configuration.
    /// </summary>
    /// <param name="configureOptions">Health check configuration delegate.</param>
    /// <returns>The current <see cref="WebApiOptions"/> instance for fluent configuration.</returns>
    public WebApiOptions UseHealthChecks(Action<HealthCheckOptions> configureOptions)
    {
        var options = new HealthCheckOptions();
        configureOptions(options);
        HealthCheckOptions = options;
        HealthCheckEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the authorization web module (<see cref="Modules.Authorization.AuthorizationWebModule"/>).
    /// </summary>
    public WebApiOptions UseUserSessions()
    {
        AuthorizationEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the Swagger web module.
    /// </summary>
    public WebApiOptions UseSwagger()
    {
        SwaggerEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables all built-in web modules at once.
    /// </summary>
    public WebApiOptions UseAllModules()
    {
        return UseLogging()
            .UseCaching()
            .UseTrafficControl()
            .UseExceptionHandling()
            .UseUserSessions()
            .UseSwagger();
    }
}