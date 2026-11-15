namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ExtendRequest;

public sealed class ExtendRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Extend, ExtendRequestHandler.Handle)
            .WithName(EndpointName.ExtendRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}