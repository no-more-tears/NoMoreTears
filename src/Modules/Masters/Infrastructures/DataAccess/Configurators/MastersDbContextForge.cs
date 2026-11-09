using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Postgres module configurator for <see cref="MastersDbContext"./>
/// </summary>
public sealed class MastersDbContextForge(
    IConfiguration configuration,
    ILoggerFactory loggerFactory,
    MastersDbContextDraft draft)
    : DbContextForge<MastersDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override DbContextBlueprint Draft(DbContextBlueprint blueprint)
    {
        draft.Apply(blueprint);
        return blueprint;
    }
}