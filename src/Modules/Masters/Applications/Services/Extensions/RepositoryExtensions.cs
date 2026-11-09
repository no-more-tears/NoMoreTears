using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services.Extensions;

/// <summary>
///     Loading of aggregates with a not found check.
/// </summary>
internal static class RepositoryExtensions
{
    /// <summary>
    ///     Loads a tracked master profile; its categories, documents and sanctions are loaded with it.
    /// </summary>
    public static async Task<Master> GetRequiredAsync(
        this IRepository<Master, Guid> masters,
        Guid masterId,
        CancellationToken cancellationToken)
    {
        return await masters.FindTrackedAsync(masterId, cancellationToken)
            ?? throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Master profile '{0}' was not found.",
                masterId));
    }

    /// <summary>
    ///     Loads a tracked master profile or returns <c>null</c>.
    /// </summary>
    public static Task<Master?> FindTrackedAsync(
        this IRepository<Master, Guid> masters,
        Guid masterId,
        CancellationToken cancellationToken)
    {
        return masters
            .AsQueryable()
            .FirstOrDefaultAsync(master => master.Id == masterId, cancellationToken);
    }

    /// <summary>
    ///     Loads a tracked category of the catalog.
    /// </summary>
    public static async Task<Category> GetRequiredAsync(
        this IRepository<Category, Guid> categories,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return await categories
            .AsQueryable()
            .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Category '{0}' was not found.",
                categoryId));
    }
}