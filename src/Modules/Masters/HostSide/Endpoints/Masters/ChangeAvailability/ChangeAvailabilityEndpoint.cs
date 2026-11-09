namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeAvailability;

public sealed class ChangeAvailabilityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Availability, ChangeAvailabilityHandler.Handle)
            .WithName(EndpointName.ChangeAvailability)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
