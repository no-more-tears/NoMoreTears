using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Extensions;
using NoMoreTears.Modules.Requests.Applications.Services.Mappings;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Common.Pagination;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Acceptance and execution of requests on behalf of masters.
/// </summary>
public sealed class ExecutionService(
    IRepository<Request, Guid> requests,
    IRepository<MasterAccess, Guid> masterAccesses,
    TimeProvider timeProvider,
    ILogger<ExecutionService> logger) : IExecutionService
{
    private const string AlreadyAcceptedMessage = "The request has already been accepted by another master.";

    /// <inheritdoc />
    public async Task<RequestResponse> GetAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);

        if (!await masterAccesses.IsVisibleToMasterAsync(request, masterId, timeProvider.GetUtcNow(), cancellationToken))
        {
            throw RepositoryExtensions.RequestNotFound(requestId);
        }

        // The address and contacts are revealed only to the master who accepted the request.
        return request.CanRevealContactsTo(masterId)
            ? request.ToResponse()
            : request.ToResponseWithoutContacts();
    }

    /// <inheritdoc />
    public async Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        Guid masterId,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var statusFilter = RepositoryExtensions.ParseStatus(status);
        var query = requests
            .AsQueryable()
            .AsNoTracking()
            .Where(request => request.Assignment!.MasterId == masterId);

        if (statusFilter is { } chosenStatus)
        {
            query = query.Where(request => request.Status == chosenStatus);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(request => request.Assignment!.AcceptedAt)
            .ThenByDescending(request => request.Id)
            .Skip(PagedResponse<RequestSummaryResponse>.Offset(pageNumber, pageSize))
            .Take(pageSize)
            .ToSummaries()
            .ToListAsync(cancellationToken);

        return new PagedResponse<RequestSummaryResponse>(items, pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);

        if (request.Assignment is null || !request.Assignment.IsAssignedTo(masterId))
        {
            throw RepositoryExtensions.RequestNotFound(requestId);
        }

        return request.ToHistoryResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> AcceptAsync(
        Guid masterId,
        Guid requestId,
        int arrivalIntervalInHours,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetRequiredAsync(requestId, cancellationToken);
        var access = await masterAccesses.FindForReadingAsync(masterId, cancellationToken)
            ?? throw new MasterNotEligibleException();
        var now = timeProvider.GetUtcNow();

        access.EnsureCanAccept(request.CategoryId, now);
        request.Accept(masterId, TimeSpan.FromHours(arrivalIntervalInHours), now);

        try
        {
            await requests.UpdateAsync(request, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another master accepted the request between reading and saving.
            throw new ConflictException(AlreadyAcceptedMessage);
        }

        logger.LogInformation("Request {RequestId} accepted by master {MasterId}", requestId, masterId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public Task<RequestResponse> MarkAsDoneAsync(
        Guid masterId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            requestId,
            (request, now) => request.MarkAsDone(masterId, now),
            "Request {RequestId} marked as done",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> RefuseAsync(
        Guid masterId,
        Guid requestId,
        string reason,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            requestId,
            (request, now) => request.Refuse(masterId, Reason.Of(reason), now),
            "Request {RequestId} refused by the master",
            cancellationToken);
    }

    private async Task<RequestResponse> ChangeAsync(
        Guid requestId,
        Action<Request, DateTimeOffset> change,
        string completedMessage,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetRequiredAsync(requestId, cancellationToken);

        change(request, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

#pragma warning disable CA2254 // The template is a constant chosen by the caller.
        logger.LogInformation(completedMessage, requestId);
#pragma warning restore CA2254

        return request.ToResponse();
    }
}