using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services.Mappings;
using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services;

/// <summary>
///     Review of master profiles by PlatformSupport.
/// </summary>
public sealed class VerificationService(
    IRepository<Master, Guid> masters,
    TimeProvider timeProvider,
    ILogger<VerificationService> logger) : IVerificationService
{
    /// <inheritdoc />
    public async Task<MasterResponse> VerifyCategoryAsync(
        Guid reviewerId,
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.VerifyCategory(reviewerId, categoryId, now),
            cancellationToken);

        logger.LogInformation(
            "Category {CategoryId} of master {MasterId} verified by {ReviewerId}",
            categoryId, masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> RejectCategoryAsync(
        Guid reviewerId,
        Guid masterId,
        Guid categoryId,
        string reason,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.RejectCategory(reviewerId, categoryId, Reason.Of(reason), now),
            cancellationToken);

        logger.LogInformation(
            "Category {CategoryId} of master {MasterId} rejected by {ReviewerId}",
            categoryId, masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> VerifyServiceAreaAsync(
        Guid reviewerId,
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.VerifyServiceArea(reviewerId, now),
            cancellationToken);

        logger.LogInformation("Service area of master {MasterId} verified by {ReviewerId}", masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> RejectServiceAreaAsync(
        Guid reviewerId,
        Guid masterId,
        string reason,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.RejectServiceArea(reviewerId, Reason.Of(reason), now),
            cancellationToken);

        logger.LogInformation("Service area of master {MasterId} rejected by {ReviewerId}", masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> VerifyAsync(
        Guid reviewerId,
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.Verify(reviewerId, now),
            cancellationToken);

        logger.LogInformation("Master {MasterId} verified by {ReviewerId}", masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> RejectAsync(
        Guid reviewerId,
        Guid masterId,
        string reason,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.Reject(reviewerId, Reason.Of(reason), now),
            cancellationToken);

        logger.LogInformation("Master {MasterId} rejected by {ReviewerId}", masterId, reviewerId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> PlaceHoldAsync(
        Guid masterId,
        string reason,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.PlaceVerificationHold(Reason.Of(reason), now),
            cancellationToken);

        logger.LogInformation("Verification of master {MasterId} placed on hold", masterId);
        return response;
    }

    /// <inheritdoc />
    public async Task<MasterResponse> ReleaseHoldAsync(
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var response = await ReviewAsync(
            masterId,
            (master, now) => master.ReleaseVerificationHold(now),
            cancellationToken);

        logger.LogInformation("Verification hold of master {MasterId} released", masterId);
        return response;
    }

    private async Task<MasterResponse> ReviewAsync(
        Guid masterId,
        Action<Master, DateTimeOffset> review,
        CancellationToken cancellationToken)
    {
        var master = await masters.GetRequiredAsync(masterId, cancellationToken);
        var now = timeProvider.GetUtcNow();

        review(master, now);
        await masters.UpdateAsync(master, cancellationToken);
        return master.ToResponse(now);
    }
}