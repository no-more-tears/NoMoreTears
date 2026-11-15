using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Postgres module configurator for <see cref="RequestsDbContext"/>.
/// </summary>
public sealed class RequestsDbContextForge(
    IConfiguration configuration,
    ILoggerFactory loggerFactory,
    RequestsDbContextDraft draft)
    : DbContextForge<RequestsDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override DbContextBlueprint Draft(DbContextBlueprint blueprint)
    {
        draft.Apply(blueprint);
        return blueprint;
    }
}