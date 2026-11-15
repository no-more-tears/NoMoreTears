namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterRequests;

public sealed class GetMasterRequestsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Base, GetMasterRequestsHandler.Handle)
            .WithName(EndpointName.GetMasterRequests)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}