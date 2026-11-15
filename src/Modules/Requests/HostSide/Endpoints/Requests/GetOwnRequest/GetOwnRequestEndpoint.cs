namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequest;

public sealed class GetOwnRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Request, GetOwnRequestHandler.Handle)
            .WithName(EndpointName.GetOwnRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}