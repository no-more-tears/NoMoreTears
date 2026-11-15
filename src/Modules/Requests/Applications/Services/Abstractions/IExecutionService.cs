using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Acceptance and execution of requests on behalf of masters.
/// </summary>
public interface IExecutionService
{
    /// <summary>
    ///     Returns a request assigned to the master, or a published request the master may accept (without the address).
    /// </summary>
    Task<RequestResponse> GetAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns a page of requests accepted by the master.
    /// </summary>
    Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        Guid masterId,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Accepts a published request; of concurrent attempts exactly one wins, the others get a conflict.
    /// </summary>
    Task<RequestResponse> AcceptAsync(
        Guid masterId,
        Guid requestId,
        int arrivalIntervalInHours,
        CancellationToken cancellationToken);

    Task<RequestResponse> MarkAsDoneAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestResponse> RefuseAsync(
        Guid masterId,
        Guid requestId,
        string reason,
        CancellationToken cancellationToken);
}