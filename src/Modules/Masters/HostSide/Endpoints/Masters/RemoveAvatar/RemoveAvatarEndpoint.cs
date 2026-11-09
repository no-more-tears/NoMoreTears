namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.RemoveAvatar;

public sealed class RemoveAvatarEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Avatar, RemoveAvatarHandler.Handle)
            .WithName(EndpointName.RemoveAvatar)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
