using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NoMoreTears.Shared.Postgres.Extensions;

public static class ModelBuilderSchemaExtensions
{
    /// <summary>
    ///     Applies the schema configured via <c>DbContextBlueprint.WithSchema</c>
    ///     as the default schema of the model. Does nothing when no schema was configured.
    /// </summary>
    /// <remarks>
    ///     Call it first in <c>OnModelCreating</c>, before entity configurations,
    ///     so that explicit <c>ToTable(name, schema)</c> calls still take precedence.
    /// </remarks>
    public static ModelBuilder UseConfiguredSchema(this ModelBuilder modelBuilder, DbContext context)
    {
        // The schema travels through the migrations history table schema set by DbContextForge.
        var schema = context
            .GetService<IDbContextOptions>()
            .Extensions
                .OfType<RelationalOptionsExtension>()
                .FirstOrDefault()?
            .MigrationsHistoryTableSchema;

        if (!string.IsNullOrWhiteSpace(schema))
        {
            modelBuilder.HasDefaultSchema(schema);
        }

        return modelBuilder;
    }
}