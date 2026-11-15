namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequest;

public sealed class GetRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Request, GetRequestHandler.Handle)
            .WithName(EndpointName.GetRequest)
            .WithTags(EndpointTag.Support)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}