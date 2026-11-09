using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Notification.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Carries the host-supplied blueprint delegate to <see cref="NotificationDbContextForge"/>.
/// </summary>
public sealed class NotificationDbContextDraft(Action<DbContextBlueprint> draft)
{
    public void Apply(DbContextBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(blueprint);

        blueprint
            .WithSchema(NotificationDbContext.Schema)
            .EnableVerboseDiagnostics(false);

        draft(blueprint);
    }
}