namespace NoMoreTears.Shared.Common.Pagination;

/// <summary>
///     A page of items with the information needed to request the next pages.
/// </summary>
/// <param name="Items">Items of the page.</param>
/// <param name="PageNumber">Number of the page, starting from 1.</param>
/// <param name="PageSize">Maximum number of items on a page.</param>
/// <param name="TotalCount">Number of items on all pages.</param>
public sealed record PagedResponse<TItem>(
    IReadOnlyList<TItem> Items,
    int PageNumber,
    int PageSize,
    long TotalCount)
{
    /// <summary>
    ///     Number of pages.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    ///     Indicates whether there is a page after this one.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    ///     Number of items to skip to read the given page.
    /// </summary>
    public static int Offset(int pageNumber, int pageSize) => (Math.Max(pageNumber, 1) - 1) * pageSize;
}
