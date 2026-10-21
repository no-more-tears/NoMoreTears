using NoMoreTears.Shared.Postgres.Configurations;

namespace NoMoreTears.Shared.Providers.Identity.Persistence;

/// <summary>
///     Carries the host-supplied blueprint delegate to <see cref="IdentityDbContextForge"/>.
/// </summary>
internal sealed class IdentityDbContextDraft(Action<DbContextBlueprint> draft)
{
    /// <summary>
    ///     Applies the delegate to the blueprint.
    /// </summary>
    public void Apply(DbContextBlueprint blueprint) => draft(blueprint);
}