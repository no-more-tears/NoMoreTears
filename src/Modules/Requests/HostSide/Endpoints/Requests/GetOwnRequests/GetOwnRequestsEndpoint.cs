namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequests;

public sealed class GetOwnRequestsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Base, GetOwnRequestsHandler.Handle)
            .WithName(EndpointName.GetOwnRequests)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}