namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.DuplicateRequest;

public sealed class DuplicateRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Duplicate, DuplicateRequestHandler.Handle)
            .WithName(EndpointName.DuplicateRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}