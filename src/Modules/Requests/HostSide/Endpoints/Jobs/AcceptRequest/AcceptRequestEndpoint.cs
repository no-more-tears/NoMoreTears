namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.AcceptRequest;

public sealed class AcceptRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Accept, AcceptRequestHandler.Handle)
            .WithName(EndpointName.AcceptRequest)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Master), nameof(UserRole.PlatformAdministrator)));
    }
}