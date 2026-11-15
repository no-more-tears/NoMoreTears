namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs.GetMasterMediaFile;

public sealed class GetMasterMediaFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.MediaFile, GetMasterMediaFileHandler.Handle)
            .WithName(EndpointName.GetMasterMediaFile)
            .WithTags(EndpointTag.Jobs)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}