namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.RefuseRequest;

public sealed class RefuseRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Refuse, RefuseRequestHandler.Handle)
            .WithName(EndpointName.RefuseRequest)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}