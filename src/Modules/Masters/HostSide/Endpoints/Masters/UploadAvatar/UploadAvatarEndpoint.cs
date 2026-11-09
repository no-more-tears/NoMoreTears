namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.UploadAvatar;

public sealed class UploadAvatarEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Avatar, UploadAvatarHandler.Handle)
            .WithName(EndpointName.UploadAvatar)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)))
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(UploadLimits.AvatarRequestSizeInBytes));
    }
}
