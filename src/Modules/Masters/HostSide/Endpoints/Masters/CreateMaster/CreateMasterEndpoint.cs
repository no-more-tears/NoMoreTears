namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.CreateMaster;

public sealed class CreateMasterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Base, CreateMasterHandler.Handle)
            .WithName(EndpointName.CreateMaster)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
