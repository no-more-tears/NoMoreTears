namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.CreateRequest;

public sealed class CreateRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Base, CreateRequestHandler.Handle)
            .WithName(EndpointName.CreateRequest)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}