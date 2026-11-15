namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.GetOwnRequestHistory;

public sealed class GetOwnRequestHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.History, GetOwnRequestHistoryHandler.Handle)
            .WithName(EndpointName.GetOwnRequestHistory)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}