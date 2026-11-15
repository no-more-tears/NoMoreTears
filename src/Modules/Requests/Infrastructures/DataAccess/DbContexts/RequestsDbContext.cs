using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Requests.Domain.Aggregates;

namespace NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     EF Core database context for the Requests.
/// </summary>
public sealed class RequestsDbContext(
    DbContextOptions<RequestsDbContext> options)
    : DbContext(options)
{
    /// <summary>
    ///     Database schema.
    /// </summary>
    public const string Schema = "requests";

    /// <summary>
    ///     Requests of clients with their photos and status history.
    /// </summary>
    public DbSet<Request> Requests => Set<Request>();

    /// <summary>
    ///     Photos waiting for or after the check.
    /// </summary>
    public DbSet<MediaUpload> MediaUploads => Set<MediaUpload>();

    /// <summary>
    ///     Publication restrictions of clients imposed by Trust.
    /// </summary>
    public DbSet<ClientRestriction> ClientRestrictions => Set<ClientRestriction>();

    /// <summary>
    ///     Snapshot of the access of masters published by Masters.
    /// </summary>
    public DbSet<MasterAccess> MasterAccesses => Set<MasterAccess>();

    /// <summary>
    ///     Snapshot of the category catalog published by Masters.
    /// </summary>
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        CustomModelBuilder.Configure(modelBuilder);
    }
}