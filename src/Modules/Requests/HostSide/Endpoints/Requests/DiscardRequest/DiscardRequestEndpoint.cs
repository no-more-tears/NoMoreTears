namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.DiscardRequest;

public sealed class DiscardRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Request, DiscardRequestHandler.Handle)
            .WithName(EndpointName.DiscardRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}