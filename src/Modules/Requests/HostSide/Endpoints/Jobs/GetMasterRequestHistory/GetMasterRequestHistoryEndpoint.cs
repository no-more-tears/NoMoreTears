namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterRequestHistory;

public sealed class GetMasterRequestHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.History, GetMasterRequestHistoryHandler.Handle)
            .WithName(EndpointName.GetMasterRequestHistory)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}