using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.Modules.Identity.Providers;

public sealed class IdentityModule : IModule
{
    public string Name => nameof(IdentityModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
    }
}