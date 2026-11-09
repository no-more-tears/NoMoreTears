namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters.SubmitForVerification;

public sealed class SubmitForVerificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.Submit, SubmitForVerificationHandler.Handle)
            .WithName(EndpointName.SubmitForVerification)
            .WithTags(EndpointTag.Masters)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Worker), nameof(UserRole.PlatformAdministrator)));
    }
}
