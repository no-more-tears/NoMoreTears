using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Identity.Applications.Services;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Configuration;
using NoMoreTears.Modules.Identity.HostSide.Endpoints;
using NoMoreTears.Shared.CQS.Registration;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;

namespace NoMoreTears.Modules.Identity.Providers;

/// <summary>
///     Registrar for identity services.
/// </summary>
public static class IdentityRegistrar
{
    /// <summary>
    ///     Registers Applications (services + handlers).
    /// </summary>
    public static IServiceCollection AddApplications(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddServices(configuration)
            .AddHandlers();

        return services;
    }

    /// <summary>
    ///     Registers identity Applications services.
    /// </summary>
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<IdentityLinkOptions>()
            .Bind(configuration.GetSection(IdentityLinkOptions.SectionName));

        services
            .AddScoped<IIdentityNotifier, IdentityNotifier>()
            .AddScoped<IIdentityService, IdentityService>()
            .AddScoped<IUserService, UserService>();

        return services;
    }

    /// <summary>
    ///     Registers identity Applications handlers.
    /// </summary>
    /// <remarks>
    ///     Payload logging is not enabled: identity requests and responses carry passwords and tokens.
    ///     Services log the outcome of each operation instead.
    /// </remarks>
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services
            .AddCqs(options => options
                .FromAssembly(Applications.Handlers.AssemblyReference.Assembly)
                .UseAuthorization()
                .UseValidation()
                .UsePerformanceTracking());

        return services;
    }

    /// <summary>
    ///     Registers identity endpoints.
    /// </summary>
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services
            .AddEndpoints(HostSide.Endpoints.AssemblyReference.Assembly);

        return services;
    }
}