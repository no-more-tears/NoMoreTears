using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="ClientRestriction"/>.
/// </summary>
public sealed class ClientRestrictionConfiguration : IEntityTypeConfiguration<ClientRestriction>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClientRestriction> builder)
    {
        builder.ToTable("ClientRestrictions");

        builder.ConfigureAuditableEntity<ClientRestriction, Guid>();

        builder.Property(restriction => restriction.ClientId).IsRequired();

        builder.Property(restriction => restriction.Kind)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.Enum)
            .IsRequired();

        builder.Property(restriction => restriction.ImposedAt).IsRequired();
        builder.Property(restriction => restriction.ExpiresAt);
        builder.Property(restriction => restriction.LiftedAt);

        builder.Ignore(restriction => restriction.IsLifted);

        builder.HasIndex(restriction => new { restriction.ClientId, restriction.LiftedAt });
    }
}