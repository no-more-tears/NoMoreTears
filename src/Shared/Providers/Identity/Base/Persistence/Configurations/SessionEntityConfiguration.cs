using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Mapping of <see cref="SessionEntity"/> to the <c>Sessions</c> table.
/// </summary>
internal sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .Property(x => x.MfaVerified)
            .HasDefaultValue(false);

        builder
            .Property(x => x.RevocationReason)
            .HasConversion<string>()
            .HasMaxLength(ColumnLengths.EnumName);

        builder
            .Property(x => x.IpAddress)
            .HasMaxLength(ColumnLengths.IpAddress);
        builder
            .Property(x => x.UserAgent)
            .HasMaxLength(ColumnLengths.UserAgent);
        builder
            .Property(x => x.DeviceId)
            .HasMaxLength(ColumnLengths.DeviceId);

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.RevokedAt });
        builder.HasIndex(x => x.ExpiresAt);
    }
}