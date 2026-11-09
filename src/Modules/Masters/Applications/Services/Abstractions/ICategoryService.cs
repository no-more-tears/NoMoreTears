using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Abstractions;

/// <summary>
///     Catalog of categories, maintained by PlatformAdministrator.
/// </summary>
public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<CategoryResponse> UpdateAsync(
        Guid categoryId,
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<CategoryResponse> ArchiveAsync(
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<CategoryResponse> ActivateAsync(
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<CategoryResponse> GetAsync(
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        bool includeArchived,
        CancellationToken cancellationToken);
}