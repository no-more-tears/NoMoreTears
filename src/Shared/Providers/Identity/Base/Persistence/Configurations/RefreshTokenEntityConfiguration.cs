using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="RefreshTokenEntity"/> to the <c>RefreshTokens</c> table.
/// </summary>
internal sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Hash);

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => x.ExpiresAt);
    }
}