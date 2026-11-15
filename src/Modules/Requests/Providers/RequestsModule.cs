using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.Modules.Requests.Providers;

/// <summary>
///     Requests module of the WebApi host.
/// </summary>
public sealed class RequestsModule : IModule
{
    private const string ConnectionStringName = "Postgres";

    public string Name => nameof(RequestsModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddRequestsDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddRequestsApplications(configuration)
            .AddRequestsEndpoints()
            .AddRequestsMessaging();
    }
}