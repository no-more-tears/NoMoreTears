using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="UserActionTokenEntity"/> to the <c>UserActionTokens</c> table.
/// </summary>
internal sealed class UserActionTokenEntityConfiguration : IEntityTypeConfiguration<UserActionTokenEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<UserActionTokenEntity> builder)
    {
        builder.ToTable("UserActionTokens");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Purpose)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.EnumName);

        builder
            .Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(ColumnLengths.Hash);

        builder
            .HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.Purpose });
        builder.HasIndex(x => x.ExpiresAt);
    }
}