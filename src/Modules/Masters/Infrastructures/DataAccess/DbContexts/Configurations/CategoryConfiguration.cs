using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Masters.Domain;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="Category"/>.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.ConfigureAuditableEntity<Category, Guid>();

        builder.Property(category => category.Name)
            .HasConversion(
                name => name.Value,
                value => CategoryName.Of(value))
            .HasMaxLength(MasterRules.MaxCategoryNameLength)
            .IsRequired();

        builder.Property(category => category.Description)
            .HasConversion(
                description => description!.Value,
                value => CategoryDescription.Of(value))
            .HasMaxLength(MasterRules.MaxCategoryDescriptionLength);

        builder.Property(category => category.Status)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Ignore(category => category.IsActive);

        // The catalog has no duplicate names among categories that were not deleted.
        builder.HasIndex(category => category.Name)
            .IsUnique()
            .HasFilter($"\"{nameof(Category.IsDeleted)}\" = false");

        builder.HasIndex(category => category.Status);
    }
}