namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestDescription;

public sealed class ChangeRequestDescriptionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Description, ChangeRequestDescriptionHandler.Handle)
            .WithName(EndpointName.ChangeRequestDescription)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}