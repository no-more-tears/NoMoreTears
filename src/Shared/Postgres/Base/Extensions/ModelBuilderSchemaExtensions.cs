using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Domain;
using System.Linq.Expressions;

namespace NoMoreTears.Shared.Postgres.Extensions;

public static class ModelBuilderSchemaExtensions
{
    extension (ModelBuilder modelBuilder)
    {
        /// <summary>
        ///     Applies the schema configured via <c>DbContextBlueprint.WithSchema</c>
        ///     as the default schema of the model. Does nothing when no schema was configured.
        /// </summary>
        /// <remarks>
        ///     Call it first in <c>OnModelCreating</c>, before entity configurations,
        ///     so that explicit <c>ToTable(name, schema)</c> calls still take precedence.
        /// </remarks>
        public ModelBuilder UseConfiguredSchema(DbContext context)
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

        /// <summary>
        ///     Applies a global query filter to all entities inheriting from <see cref="AuditableEntity{TId}"/>,
        ///     excluding soft-deleted rows (<c>IsDeleted == false</c>) from default queries.
        /// </summary>
        public ModelBuilder ApplySoftDeleteQueryFilter()
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!IsAuditableEntity(entityType.ClrType))
                {
                    continue;
                }

                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(AuditableEntity<>.IsDeleted));
                var notDeleted = Expression.Not(isDeletedProperty);
                var lambda = Expression.Lambda(notDeleted, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

            return modelBuilder;
        }
    }

    private static bool IsAuditableEntity(Type type)
    {
        var current = type;

        while (current is not null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(AuditableEntity<>))
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}