using NoMoreTears.Modules.Requests.Applications.Services.Results;
using NoMoreTears.Modules.Requests.Contracts.Requests;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Manages requests on behalf of their clients.
/// </summary>
public interface IRequestService
{
    Task<RequestResponse> CreateAsync(
        Guid clientId,
        Guid categoryId,
        AddressRequest address,
        string description,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Creates a draft with the address and category of an earlier request of the client.
    /// </summary>
    Task<RequestResponse> DuplicateAsync(
        Guid clientId,
        Guid requestId,
        string description,
        CancellationToken cancellationToken);

    Task<RequestResponse> GetAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<PagedResponse<RequestSummaryResponse>> GetPageAsync(
        Guid clientId,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StatusChangeResponse>> GetHistoryAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestResponse> ChangeCategoryAsync(
        Guid clientId,
        Guid requestId,
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<RequestResponse> ChangeAddressAsync(
        Guid clientId,
        Guid requestId,
        AddressRequest address,
        CancellationToken cancellationToken);

    Task<RequestResponse> ChangeDescriptionAsync(
        Guid clientId,
        Guid requestId,
        string description,
        CancellationToken cancellationToken);

    Task<RequestFileRemoval> DiscardAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestFileRemoval> DetachMediaFileAsync(
        Guid clientId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken);

    Task<RequestResponse> PublishAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestResponse> ExtendAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestResponse> RepublishAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Cancels a request; without a reason only before acceptance.
    /// </summary>
    Task<RequestResponse> CancelAsync(
        Guid clientId,
        Guid requestId,
        string? reason,
        CancellationToken cancellationToken);

    Task<RequestResponse> ConfirmCompletionAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);

    Task<RequestResponse> ObjectToCompletionAsync(
        Guid clientId,
        Guid requestId,
        string reason,
        CancellationToken cancellationToken);

    Task<RequestResponse> ReportNoShowAsync(
        Guid clientId,
        Guid requestId,
        CancellationToken cancellationToken);
}