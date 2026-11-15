namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ReportNoShow;

public sealed class ReportNoShowEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ReportNoShow, ReportNoShowHandler.Handle)
            .WithName(EndpointName.ReportNoShow)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}