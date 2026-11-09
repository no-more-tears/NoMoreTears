using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     Centralizes model-building for <see cref="MastersDbContext"./>
/// </summary>
public static class CustomModelBuilder
{
    /// <summary>
    ///     Applies all entity type configurations and shared model conventions.
    /// </summary>
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(MastersDbContext.Schema)
            .ApplyConfiguration(new CategoryConfiguration())
            .ApplyConfiguration(new MasterConfiguration())
            .ApplyConfiguration(new FileUploadConfiguration())
            .AddTransactionalOutbox()
            .ApplySoftDeleteQueryFilter();
    }
}