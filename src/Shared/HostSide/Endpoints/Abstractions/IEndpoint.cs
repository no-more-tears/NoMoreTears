using Microsoft.AspNetCore.Routing;

namespace NoMoreTears.Shared.HostSide.Endpoints.Abstractions;

/// <summary>
///     Interface for representing a minimal API endpoint.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    ///     Maps the endpoint's routes.
    /// </summary>
    void MapEndpoint(IEndpointRouteBuilder app);
}