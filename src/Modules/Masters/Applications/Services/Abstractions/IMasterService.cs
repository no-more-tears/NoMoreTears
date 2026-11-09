using NoMoreTears.Modules.Masters.Applications.Services.Results;
using NoMoreTears.Modules.Masters.Contracts.Requests;
using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Masters.Applications.Services.Abstractions;

/// <summary>
///     Manages master profiles on behalf of the masters themselves.
/// </summary>
public interface IMasterService
{
    Task<MasterResponse> CreateAsync(
        Guid userId,
        string firstName,
        string lastName,
        IReadOnlyCollection<Guid> categoryIds,
        ServiceAreaRequest serviceArea,
        WorkScheduleRequest workSchedule,
        CancellationToken cancellationToken);

    Task<MasterResponse> GetAsync(
        Guid masterId,
        CancellationToken cancellationToken);

    Task<PagedResponse<MasterSummaryResponse>> GetPageAsync(
        string? verification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<MasterResponse> ChangeFullNameAsync(
        Guid masterId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken);

    Task<MasterResponse> ChangeServiceAreaAsync(
        Guid masterId,
        ServiceAreaRequest serviceArea,
        CancellationToken cancellationToken);

    Task<MasterResponse> ChangeWorkScheduleAsync(
        Guid masterId,
        WorkScheduleRequest workSchedule,
        CancellationToken cancellationToken);

    Task<MasterResponse> ChangeAvailabilityAsync(
        Guid masterId,
        bool isAvailable,
        CancellationToken cancellationToken);

    Task<MasterResponse> AddCategoryAsync(
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<MasterResponse> RemoveCategoryAsync(
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<MasterResponse> SubmitForVerificationAsync(
        Guid masterId,
        CancellationToken cancellationToken);

    Task<MasterFileRemoval> RemoveAvatarAsync(
        Guid masterId,
        CancellationToken cancellationToken);

    Task<MasterFileRemoval> RemoveDocumentAsync(
        Guid masterId,
        Guid documentId,
        CancellationToken cancellationToken);
}