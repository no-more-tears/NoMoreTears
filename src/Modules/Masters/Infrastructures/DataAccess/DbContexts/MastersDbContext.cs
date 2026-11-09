using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Masters.Domain.Aggregates;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     EF Core database context for the Masters.
/// </summary>
public sealed class MastersDbContext(
    DbContextOptions<MastersDbContext> options)
    : DbContext(options)
{
    /// <summary>
    ///     Database schema.
    /// </summary>
    public const string Schema = "masters";

    /// <summary>
    ///     Master profiles.
    /// </summary>
    public DbSet<Master> Masters => Set<Master>();

    /// <summary>
    ///     Catalog of categories.
    /// </summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    ///     Files waiting for or after the check.
    /// </summary>
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        CustomModelBuilder.Configure(modelBuilder);
    }
}