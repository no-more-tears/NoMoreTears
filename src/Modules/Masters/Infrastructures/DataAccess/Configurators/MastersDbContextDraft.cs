using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Carries the host-supplied blueprint delegate to <see cref="MastersDbContextForge"/>.
/// </summary>
public sealed class MastersDbContextDraft(Action<DbContextBlueprint> draft)
{
    public void Apply(DbContextBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(blueprint);

        blueprint
            .WithSchema(MastersDbContext.Schema)
            .EnableVerboseDiagnostics(false);

        draft(blueprint);
    }
}