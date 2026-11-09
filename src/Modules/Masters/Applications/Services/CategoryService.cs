using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services.Mappings;
using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services;

/// <summary>
///     Catalog of categories, maintained by PlatformAdministrator.
/// </summary>
public sealed class CategoryService(
    IRepository<Category, Guid> categories,
    TimeProvider timeProvider,
    ILogger<CategoryService> logger) : ICategoryService
{
    /// <inheritdoc />
    public async Task<CategoryResponse> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var categoryName = CategoryName.Of(name);
        await EnsureNameIsFreeAsync(categoryName, null, cancellationToken);

        var category = Category.Create(categoryName, ToDescription(description), timeProvider.GetUtcNow());
        await categories.CreateAsync(category, cancellationToken);

        logger.LogInformation("Category {CategoryId} created", category.Id);
        return category.ToResponse();
    }

    /// <inheritdoc />
    public async Task<CategoryResponse> UpdateAsync(
        Guid categoryId,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetRequiredAsync(categoryId, cancellationToken);
        var categoryName = CategoryName.Of(name);
        var now = timeProvider.GetUtcNow();

        if (category.Name != categoryName)
        {
            await EnsureNameIsFreeAsync(categoryName, categoryId, cancellationToken);
            category.Rename(categoryName, now);
        }

        category.ChangeDescription(ToDescription(description), now);
        await categories.UpdateAsync(category, cancellationToken);

        logger.LogInformation("Category {CategoryId} updated", categoryId);
        return category.ToResponse();
    }

    /// <inheritdoc />
    public async Task<CategoryResponse> ArchiveAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetRequiredAsync(categoryId, cancellationToken);

        category.Archive(timeProvider.GetUtcNow());
        await categories.UpdateAsync(category, cancellationToken);

        logger.LogInformation("Category {CategoryId} archived", categoryId);

        return category.ToResponse();
    }

    /// <inheritdoc />
    public async Task<CategoryResponse> ActivateAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetRequiredAsync(categoryId, cancellationToken);

        category.Activate(timeProvider.GetUtcNow());
        await categories.UpdateAsync(category, cancellationToken);

        logger.LogInformation("Category {CategoryId} activated", categoryId);

        return category.ToResponse();
    }

    /// <inheritdoc />
    public async Task<CategoryResponse> GetAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetRequiredAsync(categoryId, cancellationToken);
        return category.ToResponse();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        bool includeArchived,
        CancellationToken cancellationToken)
    {
        var query = categories.AsQueryable().AsNoTracking();

        if (!includeArchived)
        {
            query = query.Where(category => category.Status == CategoryStatus.Active);
        }

        var found = await query.ToListAsync(cancellationToken);

        return [.. found
            .OrderBy(category => category.Name.Value, StringComparer.CurrentCultureIgnoreCase)
            .Select(category => category.ToResponse())];
    }

    private async Task EnsureNameIsFreeAsync(
        CategoryName name,
        Guid? exceptCategoryId,
        CancellationToken cancellationToken)
    {
        var isTaken = await categories.ExistsAsync(
            category => category.Name == name && category.Id != exceptCategoryId,
            cancellationToken);

        if (isTaken)
        {
            throw new ConflictException($"Category '{name.Value}' already exists.");
        }
    }

    private static CategoryDescription? ToDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : CategoryDescription.Of(description);
}