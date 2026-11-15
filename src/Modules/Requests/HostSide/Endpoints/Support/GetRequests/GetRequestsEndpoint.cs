namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequests;

public sealed class GetRequestsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Base, GetRequestsHandler.Handle)
            .WithName(EndpointName.GetRequests)
            .WithTags(EndpointTag.Support)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}