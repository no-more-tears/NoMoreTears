using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Postgres module configurator for <see cref="NotificationDbContext"./>
/// </summary>
public sealed class NotificationDbContextForge(
    IConfiguration configuration,
    ILoggerFactory loggerFactory,
    NotificationDbContextDraft draft)
    : DbContextForge<NotificationDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override DbContextBlueprint Draft(DbContextBlueprint blueprint)
    {
        draft.Apply(blueprint);
        return blueprint;
    }
}