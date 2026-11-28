namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.MarkRequestAsDone;

public sealed class MarkRequestAsDoneEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Done, MarkRequestAsDoneHandler.Handle)
            .WithName(EndpointName.MarkRequestAsDone)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Master), nameof(UserRole.PlatformAdministrator)));
    }
}