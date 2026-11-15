using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Carries the host-supplied blueprint delegate to <see cref="RequestsDbContextForge"/>.
/// </summary>
public sealed class RequestsDbContextDraft(Action<DbContextBlueprint> draft)
{
    public void Apply(DbContextBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(blueprint);

        blueprint
            .WithSchema(RequestsDbContext.Schema)
            .EnableVerboseDiagnostics(false);

        draft(blueprint);
    }
}