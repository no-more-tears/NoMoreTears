namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.GetAvatar;

public sealed class GetAvatarEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.MasterAvatar, GetAvatarHandler.Handle)
            .WithName(EndpointName.GetAvatar)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization();
    }
}
