using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services.Extensions;

/// <summary>
///     Loading of aggregates with a not found check.
/// </summary>
internal static class RepositoryExtensions
{
    /// <summary>
    ///     Loads a tracked request; its photos and status history are loaded with it.
    /// </summary>
    public static async Task<Request> GetRequiredAsync(
        this IRepository<Request, Guid> requests,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return await requests.FindTrackedAsync(requestId, cancellationToken)
            ?? throw RequestNotFound(requestId);
    }

    /// <summary>
    ///     Loads a tracked request.
    /// </summary>
    public static Task<Request?> FindTrackedAsync(
        this IRepository<Request, Guid> requests,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return requests
            .AsQueryable()
            .FirstOrDefaultAsync(request => request.Id == requestId, cancellationToken);
    }

    /// <summary>
    ///     Loads a tracked request of the client; a request of another client is reported as not found.
    /// </summary>
    public static async Task<Request> GetOwnedAsync(
        this IRepository<Request, Guid> requests,
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return await requests
            .AsQueryable()
            .FirstOrDefaultAsync(request => request.Id == requestId && request.ClientId == clientId, cancellationToken)
            ?? throw RequestNotFound(requestId);
    }

    /// <summary>
    ///     Loads a request for reading.
    /// </summary>
    public static async Task<Request> GetForReadingAsync(
        this IRepository<Request, Guid> requests,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return await requests
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == requestId, cancellationToken)
            ?? throw RequestNotFound(requestId);
    }

    /// <summary>
    ///     Loads a request of the client for reading.
    /// </summary>
    public static async Task<Request> GetOwnedForReadingAsync(
        this IRepository<Request, Guid> requests,
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return await requests
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == requestId && request.ClientId == clientId, cancellationToken)
            ?? throw RequestNotFound(requestId);
    }

    /// <summary>
    ///     Loads a category of the catalog snapshot and ensures it can be chosen.
    /// </summary>
    public static async Task<ServiceCategory> GetChoosableAsync(
        this IRepository<ServiceCategory, Guid> categories,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categories
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(found => found.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Category '{0}' was not found.",
                categoryId));

        category.EnsureCanBeChosen();
        return category;
    }

    /// <summary>
    ///     Loads the access snapshot of a master for reading.
    /// </summary>
    public static Task<MasterAccess?> FindForReadingAsync(
        this IRepository<MasterAccess, Guid> masterAccesses,
        Guid masterId,
        CancellationToken cancellationToken)
    {
        return masterAccesses
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(access => access.Id == masterId, cancellationToken);
    }

    /// <summary>
    ///     Returns whether a master may see a request: the request is assigned to them, or it is published and
    ///     the master may accept its category.
    /// </summary>
    public static async Task<bool> IsVisibleToMasterAsync(
        this IRepository<MasterAccess, Guid> masterAccesses,
        Request request,
        Guid masterId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Assignment is not null && request.Assignment.IsAssignedTo(masterId))
        {
            return true;
        }

        if (request.Status is not RequestStatus.Published)
        {
            return false;
        }

        var access = await masterAccesses.FindForReadingAsync(masterId, cancellationToken);
        return access is not null && access.CanAcceptAt(request.CategoryId, now);
    }

    /// <summary>
    ///     Parses an optional status filter.
    /// </summary>
    public static RequestStatus? ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return Enum.TryParse<RequestStatus>(status, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : throw new BadRequestException($"Request status '{status}' is unknown.");
    }

    /// <summary>
    ///     Projects a page of requests to summaries.
    /// </summary>
    public static IQueryable<RequestSummaryResponse> ToSummaries(this IQueryable<Request> query) =>
        query.Select(request => new RequestSummaryResponse(
            request.Id,
            request.ClientId,
            request.CategoryId,
            request.Status.ToString(),
            request.PublicationCount,
            request.PublishedAt,
            request.AcceptanceDeadline,
            (Guid?)request.Assignment!.MasterId,
            (DateTimeOffset?)request.Assignment!.ExpectedArrivalAt,
            request.CreatedAt,
            request.LastUpdatedAt));

    public static NotFoundException RequestNotFound(Guid requestId) =>
        new(string.Format(CultureInfo.InvariantCulture, "Request '{0}' was not found.", requestId));
}