using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Authorization;

/// <summary>
///     Registers <see cref="Common.Authorization.ICurrentUserSession"/>
///     resolved from the current HTTP request.
/// </summary>
public sealed class AuthorizationWebModule : IWebModule
{
    public int Order => WebModuleOrder.Authorization;

    public void MountServices(WebModuleContext context)
    {
        context.Services
            .AddHttpContextAccessor()
            .AddUserSessions(HttpUserSessionFactory.Create);
    }

    public void MountPipeline(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}