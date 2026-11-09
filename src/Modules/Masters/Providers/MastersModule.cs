using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.Modules.Masters.Providers;

/// <summary>
///     Masters module of the WebApi host.
/// </summary>
public sealed class MastersModule : IModule
{
    private const string ConnectionStringName = "Postgres";

    public string Name => nameof(MastersModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddMastersDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddMastersApplications(configuration)
            .AddMastersEndpoints()
            .AddMastersMessaging();
    }
}