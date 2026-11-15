namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.CancelRequest;

public sealed class CancelRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Cancel, CancelRequestHandler.Handle)
            .WithName(EndpointName.CancelRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}