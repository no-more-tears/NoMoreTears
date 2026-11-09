using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Abstractions;

/// <summary>
///     Review of master profiles by PlatformSupport.
/// </summary>
public interface IVerificationService
{
    Task<MasterResponse> VerifyCategoryAsync(
        Guid reviewerId,
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken);

    Task<MasterResponse> RejectCategoryAsync(
        Guid reviewerId,
        Guid masterId,
        Guid categoryId,
        string reason,
        CancellationToken cancellationToken);

    Task<MasterResponse> VerifyServiceAreaAsync(
        Guid reviewerId,
        Guid masterId,
        CancellationToken cancellationToken);

    Task<MasterResponse> RejectServiceAreaAsync(
        Guid reviewerId,
        Guid masterId,
        string reason,
        CancellationToken cancellationToken);

    Task<MasterResponse> VerifyAsync(
        Guid reviewerId,
        Guid masterId,
        CancellationToken cancellationToken);

    Task<MasterResponse> RejectAsync(
        Guid reviewerId,
        Guid masterId,
        string reason,
        CancellationToken cancellationToken);

    Task<MasterResponse> PlaceHoldAsync(
        Guid masterId,
        string reason,
        CancellationToken cancellationToken);

    Task<MasterResponse> ReleaseHoldAsync(
        Guid masterId,
        CancellationToken cancellationToken);
}