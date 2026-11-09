namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeWorkSchedule;

public sealed class ChangeWorkScheduleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.WorkSchedule, ChangeWorkScheduleHandler.Handle)
            .WithName(EndpointName.ChangeWorkSchedule)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
