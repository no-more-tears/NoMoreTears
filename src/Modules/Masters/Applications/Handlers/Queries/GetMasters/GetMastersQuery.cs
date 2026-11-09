using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasters;

/// <summary>
///     Returns a page of master profiles, optionally filtered by verification status, oldest submission first.
/// </summary>
public sealed record GetMastersQuery(
    string? Verification,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<MasterSummaryResponse>>, IPagedQuery;
