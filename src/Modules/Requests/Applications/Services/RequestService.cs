using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Configuration;
using NoMoreTears.Modules.Requests.Applications.Services.Extensions;
using NoMoreTears.Modules.Requests.Applications.Services.Mappings;
using NoMoreTears.Modules.Requests.Applications.Services.Results;
using NoMoreTears.Modules.Requests.Contracts.Requests;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Pagination;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Manages requests on behalf of their clients.
/// </summary>
public sealed class RequestService(
    IRepository<Request, Guid> requests,
    IRepository<ServiceCategory, Guid> categories,
    IRepository<ClientRestriction, Guid> restrictions,
    IOptions<RequestsStorageOptions> storageOptions,
    TimeProvider timeProvider,
    ILogger<RequestService> logger) : IRequestService
{
    private readonly RequestsStorageOptions _storage = storageOptions.Value;

    /// <inheritdoc />
    public async Task<RequestResponse> CreateAsync(
        Guid clientId,
        Guid categoryId,
        AddressRequest address,
        string description,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(address);

        await categories.GetChoosableAsync(categoryId, cancellationToken);

        var request = Request.Create(
            clientId,
            categoryId,
            address.ToAddress(),
            Description.Of(description),
            timeProvider.GetUtcNow());

        await requests.CreateAsync(request, cancellationToken);

        logger.LogInformation("Request {RequestId} of client {ClientId} created", request.Id, clientId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> DuplicateAsync(
        Guid clientId,
        Guid requestId,
        string description,
        CancellationToken cancellationToken)
    {
        var source = await requests.GetOwnedForReadingAsync(clientId, requestId, cancellationToken);

        await categories.GetChoosableAsync(source.CategoryId, cancellationToken);

        var address = Address.Of(
            source.Address.Value,
            Coordinates.Of(source.Address.Coordinates.Latitude, source.Address.Coordinates.Longitude));

        var request = Request.Create(
            clientId,
            source.CategoryId,
            address,
            Description.Of(description),
            timeProvider.GetUtcNow());

        await requests.CreateAsync(request, cancellationToken);

        logger.LogInformation("Request {RequestId} created from request {SourceRequestId}", request.Id, requestId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> GetAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedForReadingAsync(clientId, requestId, cancellationToken);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        Guid clientId,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var statusFilter = RepositoryExtensions.ParseStatus(status);
        var query = requests
            .AsQueryable()
            .AsNoTracking()
            .Where(request => request.ClientId == clientId);

        if (statusFilter is { } chosenStatus)
        {
            query = query.Where(request => request.Status == chosenStatus);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(request => request.CreatedAt)
            .ThenByDescending(request => request.Id)
            .Skip(PagedResponse<RequestSummaryResponse>.Offset(pageNumber, pageSize))
            .Take(pageSize)
            .ToSummaries()
            .ToListAsync(cancellationToken);

        return new PagedResponse<RequestSummaryResponse>(items, pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedForReadingAsync(clientId, requestId, cancellationToken);
        return request.ToHistoryResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> ChangeCategoryAsync(
        Guid clientId,
        Guid requestId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        await categories.GetChoosableAsync(categoryId, cancellationToken);

        return await ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ChangeCategory(clientId, categoryId, now),
            "Category of request {RequestId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> ChangeAddressAsync(
        Guid clientId,
        Guid requestId,
        AddressRequest address,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(address);

        return ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ChangeAddress(clientId, address.ToAddress(), now),
            "Address of request {RequestId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> ChangeDescriptionAsync(
        Guid clientId,
        Guid requestId,
        string description,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ChangeDescription(clientId, Description.Of(description), now),
            "Description of request {RequestId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RequestFileRemoval> DiscardAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);
        var files = request.MediaFiles
            .Select(mediaFile => new StoredFileResponse(_storage.MediaBucket, mediaFile.StorageKey))
            .ToList();

        request.Discard(clientId, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Draft request {RequestId} discarded", requestId);
        return new RequestFileRemoval(request.ToResponse(), files);
    }

    /// <inheritdoc />
    public async Task<RequestFileRemoval> DetachMediaFileAsync(
        Guid clientId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);
        var mediaFile = request.MediaFiles.FirstOrDefault(attached => attached.Id == mediaFileId)
            ?? throw new MediaFileNotFoundException(mediaFileId);

        request.DetachMediaFile(clientId, mediaFileId, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Media file {MediaFileId} detached from request {RequestId}", mediaFileId, requestId);

        return new RequestFileRemoval(
            request.ToResponse(),
            [new StoredFileResponse(_storage.MediaBucket, mediaFile.StorageKey)]);
    }

    /// <inheritdoc />
    public async Task<RequestResponse> PublishAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);
        var now = timeProvider.GetUtcNow();

        if (request.Status is RequestStatus.Draft)
        {
            await categories.GetChoosableAsync(request.CategoryId, cancellationToken);
        }

        var (policy, publishedCount) = await ResolvePublicationAsync(clientId, now, cancellationToken);

        request.Publish(clientId, policy, publishedCount, now);
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Request {RequestId} published", requestId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> ExtendAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var (policy, publishedCount) = await ResolvePublicationAsync(clientId, now, cancellationToken);

        request.Extend(clientId, policy, publishedCount, now);
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Waiting for acceptance of request {RequestId} extended", requestId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public async Task<RequestResponse> RepublishAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);
        var now = timeProvider.GetUtcNow();

        if (request.Status is RequestStatus.Failed)
        {
            await categories.GetChoosableAsync(request.CategoryId, cancellationToken);
        }

        var (policy, publishedCount) = await ResolvePublicationAsync(clientId, now, cancellationToken);

        request.Republish(clientId, policy, publishedCount, now);
        await requests.UpdateAsync(request, cancellationToken);

        logger.LogInformation("Request {RequestId} republished", requestId);
        return request.ToResponse();
    }

    /// <inheritdoc />
    public Task<RequestResponse> CancelAsync(
        Guid clientId,
        Guid requestId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var cancellationReason = reason.ToOptionalReason();

        return ChangeAsync(
            clientId,
            requestId,
            (request, now) =>
            {
                if (cancellationReason is null)
                {
                    request.Cancel(clientId, now);
                }
                else
                {
                    request.Cancel(clientId, cancellationReason, now);
                }
            },
            "Request {RequestId} cancelled",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> ConfirmCompletionAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ConfirmCompletion(clientId, now),
            "Completion of request {RequestId} confirmed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> ObjectToCompletionAsync(
        Guid clientId,
        Guid requestId,
        string reason,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ObjectToCompletion(clientId, Reason.Of(reason), now),
            "Completion of request {RequestId} objected",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<RequestResponse> ReportNoShowAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            clientId,
            requestId,
            (request, now) => request.ReportNoShow(clientId, now),
            "No-show reported for request {RequestId}",
            cancellationToken);
    }

    private async Task<RequestResponse> ChangeAsync(
        Guid clientId,
        Guid requestId,
        Action<Request, DateTimeOffset> change,
        string completedMessage,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedAsync(clientId, requestId, cancellationToken);

        change(request, timeProvider.GetUtcNow());
        await requests.UpdateAsync(request, cancellationToken);

#pragma warning disable CA2254 // The template is a constant chosen by the caller.
        logger.LogInformation(completedMessage, requestId);
#pragma warning restore CA2254

        return request.ToResponse();
    }

    private async Task<(PublicationPolicy Policy, int PublishedCount)> ResolvePublicationAsync(
        Guid clientId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var clientRestrictions = await restrictions
            .AsQueryable()
            .AsNoTracking()
            .Where(restriction => restriction.ClientId == clientId && restriction.LiftedAt == null)
            .ToListAsync(cancellationToken);

        var policy = PublicationPolicy.ForClient(clientRestrictions, now);

        var publishedCount = await requests.CountAsync(
            request => request.ClientId == clientId && request.Status == RequestStatus.Published,
            cancellationToken);

        return (policy, Convert.ToInt32(publishedCount, CultureInfo.InvariantCulture));
    }
}