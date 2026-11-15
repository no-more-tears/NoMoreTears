using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Extensions;
using NoMoreTears.Modules.Requests.Applications.Services.Mappings;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Common.Pagination;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Access of arbiters to requests and disputes opened by Trust.
/// </summary>
public sealed class ArbitrationService(
    IRepository<Request, Guid> requests,
    TimeProvider timeProvider,
    ILogger<ArbitrationService> logger) : IArbitrationService
{
    /// <inheritdoc />
    public async Task<RequestResponse> GetAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        string? status,
        Guid? clientId,
        Guid? masterId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var statusFilter = RepositoryExtensions.ParseStatus(status);
        var query = requests.AsQueryable().AsNoTracking();

        if (statusFilter is { } chosenStatus)
        {
            query = query.Where(request => request.Status == chosenStatus);
        }
        if (clientId is { } chosenClientId)
        {
            query = query.Where(request => request.ClientId == chosenClientId);
        }
        if (masterId is { } chosenMasterId)
        {
            query = query.Where(request => request.Assignment!.MasterId == chosenMasterId);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // The latest changes come first.
        var items = await query
            .OrderByDescending(request => request.LastUpdatedAt ?? request.CreatedAt)
            .ThenByDescending(request => request.Id)
            .Skip(PagedResponse<RequestSummaryResponse>.Offset(pageNumber, pageSize))
            .Take(pageSize)
            .ToSummaries()
            .ToListAsync(cancellationToken);

        return new PagedResponse<RequestSummaryResponse>(items, pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);
        return request.ToHistoryResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse?> DisputeAsync(
        Guid requestId,
        Guid incidentId,
        CancellationToken cancellationToken)
    {
        var request = await requests.FindTrackedAsync(requestId, cancellationToken);

        // Unknown request or redelivered message.
        if (request is null || request.IncidentId == incidentId)
        {
            return null;
        }

        request.Dispute(incidentId, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Request {RequestId} disputed by incident {IncidentId}", requestId, incidentId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse?> SettleDisputeAsync(
        Guid requestId,
        Guid incidentId,
        CancellationToken cancellationToken)
    {
        var request = await requests.FindTrackedAsync(requestId, cancellationToken);

        // The incident did not dispute the request or the dispute is already settled.
        if (request is null || request.Status is not RequestStatus.Disputed || request.IncidentId != incidentId)
        {
            return null;
        }

        request.SettleDispute(incidentId, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Dispute of request {RequestId} settled by incident {IncidentId}", requestId, incidentId);
        return request.ToResponse();
    }
}