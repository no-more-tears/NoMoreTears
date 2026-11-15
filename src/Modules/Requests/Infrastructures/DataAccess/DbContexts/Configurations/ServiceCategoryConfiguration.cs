using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Requests.Domain;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for the <see cref="ServiceCategory"/> snapshot.
/// </summary>
public sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories");

        builder.ConfigureAuditableEntity<ServiceCategory, Guid>();

        builder.Property(category => category.Name)
            .HasMaxLength(RequestRules.MaxCategoryNameLength)
            .IsRequired();

        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.SnapshotAt).IsRequired();
    }
}