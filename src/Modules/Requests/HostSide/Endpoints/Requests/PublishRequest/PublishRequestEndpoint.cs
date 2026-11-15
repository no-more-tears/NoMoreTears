namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.PublishRequest;

public sealed class PublishRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Publish, PublishRequestHandler.Handle)
            .WithName(EndpointName.PublishRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}