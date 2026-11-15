namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterRequest;

public sealed class GetMasterRequestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Job, GetMasterRequestHandler.Handle)
            .WithName(EndpointName.GetMasterRequest)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}