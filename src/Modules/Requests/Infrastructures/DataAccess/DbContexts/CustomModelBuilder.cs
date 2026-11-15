using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     Centralizes model-building for <see cref="RequestsDbContext"/>.
/// </summary>
public static class CustomModelBuilder
{
    /// <summary>
    ///     Applies all entity type configurations and shared model conventions.
    /// </summary>
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(RequestsDbContext.Schema)
            .ApplyConfiguration(new RequestConfiguration())
            .ApplyConfiguration(new MediaUploadConfiguration())
            .ApplyConfiguration(new ClientRestrictionConfiguration())
            .ApplyConfiguration(new MasterAccessConfiguration())
            .ApplyConfiguration(new ServiceCategoryConfiguration())
            .AddTransactionalOutbox()
            .ApplySoftDeleteQueryFilter();
    }
}