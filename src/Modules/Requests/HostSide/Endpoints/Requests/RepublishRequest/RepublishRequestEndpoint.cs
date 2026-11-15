namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.RepublishRequest;

public sealed class RepublishRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Republish, RepublishRequestHandler.Handle)
            .WithName(EndpointName.RepublishRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}