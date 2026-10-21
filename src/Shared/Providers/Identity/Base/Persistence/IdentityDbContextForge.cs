using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Shared.Providers.Identity.Persistence;

/// <summary>
///     Postgres module configurator for <see cref="IdentityDbContext"/> whose blueprint
///     is drafted by a delegate supplied at registration.
/// </summary>
internal sealed class IdentityDbContextForge(
    IConfiguration configuration,
    ILoggerFactory loggerFactory,
    IdentityDbContextDraft draft)
    : DbContextForge<IdentityDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override DbContextBlueprint Draft(DbContextBlueprint blueprint)
    {
        draft.Apply(blueprint);
        return blueprint;
    }
}