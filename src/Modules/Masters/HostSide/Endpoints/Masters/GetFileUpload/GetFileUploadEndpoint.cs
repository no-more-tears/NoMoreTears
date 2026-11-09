namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetFileUpload;

public sealed class GetFileUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Upload, GetFileUploadHandler.Handle)
            .WithName(EndpointName.GetFileUpload)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
