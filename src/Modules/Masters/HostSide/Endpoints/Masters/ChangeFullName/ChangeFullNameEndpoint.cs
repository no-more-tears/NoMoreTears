namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.ChangeFullName;

public sealed class ChangeFullNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.FullName, ChangeFullNameHandler.Handle)
            .WithName(EndpointName.ChangeFullName)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
