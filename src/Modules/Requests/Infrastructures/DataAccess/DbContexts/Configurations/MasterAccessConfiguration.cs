using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Extensions;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for the <see cref="MasterAccess"/> snapshot.
/// </summary>
public sealed class MasterAccessConfiguration : IEntityTypeConfiguration<MasterAccess>
{
    private const string AcceptableCategoryIdsField = "_acceptableCategoryIds";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<MasterAccess> builder)
    {
        builder.ToTable("MasterAccesses");

        builder.ConfigureAuditableEntity<MasterAccess, Guid>();

        builder.Ignore(access => access.AcceptableCategoryIds);

        builder.PrimitiveCollection<List<Guid>>(AcceptableCategoryIdsField)
            .HasColumnName("AcceptableCategoryIds")
            .IsRequired();

        builder.Property(access => access.SuspendedUntil);
        builder.Property(access => access.SnapshotAt).IsRequired();
    }
}