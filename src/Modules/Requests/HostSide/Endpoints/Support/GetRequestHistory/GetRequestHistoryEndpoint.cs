namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support.GetRequestHistory;

public sealed class GetRequestHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.History, GetRequestHistoryHandler.Handle)
            .WithName(EndpointName.GetRequestHistory)
            .WithTags(EndpointTag.Support)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.PlatformSupport), nameof(UserRole.PlatformAdministrator)));
    }
}