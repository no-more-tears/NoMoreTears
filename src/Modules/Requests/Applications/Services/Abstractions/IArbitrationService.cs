using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Access of arbiters to requests and disputes opened by Trust.
/// </summary>
public interface IArbitrationService
{
    Task<RequestResponse> GetAsync(
        Guid requestId,
        CancellationToken cancellationToken);

    Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        string? status,
        Guid? clientId,
        Guid? masterId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid requestId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Moves the request to dispute.
    /// </summary>
    Task<RequestResponse?> DisputeAsync(
        Guid requestId,
        Guid incidentId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Ends the dispute.
    /// </summary>
    Task<RequestResponse?> SettleDisputeAsync(
        Guid requestId,
        Guid incidentId,
        CancellationToken cancellationToken);
}