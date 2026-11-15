namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests.ChangeRequestAddress;

public sealed class ChangeRequestAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.Address, ChangeRequestAddressHandler.Handle)
            .WithName(EndpointName.ChangeRequestAddress)
            .WithTags(EndpointTag.Requests)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Client), nameof(UserRole.PlatformAdministrator)));
    }
}