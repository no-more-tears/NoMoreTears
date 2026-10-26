using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

namespace NoMoreTears.Shared.HostSide.Endpoints.Registration;

/// <summary>
///     Registrar for minimal API endpoints.
/// </summary>
public static class EndpointRegistrar
{
    /// <summary>
    ///     Registers all <see cref="IEndpoint"/> implementations from the specified assembly.
    /// </summary>
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var descriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(descriptors);

        return services;
    }

    /// <summary>
    ///     Maps all registered <see cref="IEndpoint"/> implementations,
    ///     optionally under the specified route group.
    /// </summary>
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var builder = routeGroupBuilder ?? app;

        foreach (var endpoint in app.ServiceProvider.GetServices<IEndpoint>())
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}