using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Configuration;
using NoMoreTears.Modules.Masters.Applications.Services.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services.Mappings;
using NoMoreTears.Modules.Masters.Applications.Services.Results;
using NoMoreTears.Modules.Masters.Contracts.Requests;
using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Common.Pagination;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services;

/// <summary>
///     Manages master profiles on behalf of the masters themselves.
/// </summary>
public sealed class MasterService(
    IRepository<Master, Guid> masters,
    IRepository<Category, Guid> categories,
    IOptions<MastersStorageOptions> storageOptions,
    TimeProvider timeProvider,
    ILogger<MasterService> logger) : IMasterService
{
    private readonly MastersStorageOptions _storage = storageOptions.Value;

    /// <inheritdoc />
    public async Task<MasterResponse> CreateAsync(
        Guid userId,
        string firstName,
        string lastName,
        IReadOnlyCollection<Guid> categoryIds,
        ServiceAreaRequest serviceArea,
        WorkScheduleRequest workSchedule,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(categoryIds);

        var exists = await masters
            .AsQueryable()
            .IgnoreQueryFilters()
            .AnyAsync(master => master.Id == userId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("The user already has a master profile.");
        }

        var chosenCategories = await LoadCategoriesAsync(categoryIds, cancellationToken);
        var now = timeProvider.GetUtcNow();

        var master = Master.Create(
            userId,
            FullName.Of(firstName, lastName),
            chosenCategories,
            serviceArea.ToServiceArea(),
            workSchedule.ToWorkSchedule(),
            now);

        await masters.CreateAsync(master, cancellationToken);

        logger.LogInformation("Master profile {MasterId} created", master.Id);
        return master.ToResponse(now);
    }

    /// <inheritdoc />
    public async Task<MasterResponse> GetAsync(
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var master = await masters
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Id == masterId, cancellationToken)
            ?? throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Master profile '{0}' was not found.",
                masterId));

        return master.ToResponse(timeProvider.GetUtcNow());
    }

    /// <inheritdoc />
    public async Task<PagedResponse<MasterSummaryResponse>> GetPageAsync(
        string? verification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = masters.AsQueryable().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(verification))
        {
            if (!Enum.TryParse<VerificationStatus>(verification, ignoreCase: true, out var status))
            {
                throw new BadRequestException($"Verification status '{verification}' is unknown.");
            }

            query = query.Where(master => master.Verification == status);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // Reviewers work the queue from the oldest submission.
        var items = await query
            .OrderBy(master => master.SubmittedAt ?? master.CreatedAt)
            .ThenBy(master => master.Id)
            .Skip(PagedResponse<MasterSummaryResponse>.Offset(pageNumber, pageSize))
            .Take(pageSize)
            .Select(master => new MasterSummaryResponse(
                master.Id,
                master.FullName.FirstName,
                master.FullName.LastName,
                master.Verification.ToString(),
                master.ServiceAreaStatus.ToString(),
                master.SubmittedAt,
                master.VerificationHold != null,
                master.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResponse<MasterSummaryResponse>(items, pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc />
    public Task<MasterResponse> ChangeFullNameAsync(
        Guid masterId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            masterId,
            (master, now) => master.ChangeFullName(FullName.Of(firstName, lastName), now),
            "Full name of master {MasterId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<MasterResponse> ChangeServiceAreaAsync(
        Guid masterId,
        ServiceAreaRequest serviceArea,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(serviceArea);

        return ChangeAsync(
            masterId,
            (master, now) => master.ChangeServiceArea(serviceArea.ToServiceArea(), now),
            "Service area of master {MasterId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<MasterResponse> ChangeWorkScheduleAsync(
        Guid masterId,
        WorkScheduleRequest workSchedule,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(workSchedule);

        return ChangeAsync(
            masterId,
            (master, now) => master.ChangeWorkSchedule(workSchedule.ToWorkSchedule(), now),
            "Work schedule of master {MasterId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<MasterResponse> ChangeAvailabilityAsync(
        Guid masterId,
        bool isAvailable,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            masterId,
            (master, now) =>
            {
                if (isAvailable)
                {
                    master.BecomeAvailable(now);
                }
                else
                {
                    master.BecomeUnavailable(now);
                }
            },
            "Availability of master {MasterId} changed",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MasterResponse> AddCategoryAsync(
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetRequiredAsync(categoryId, cancellationToken);

        return await ChangeAsync(
            masterId,
            (master, now) => master.AddCategory(category, now),
            "Category added to master {MasterId}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<MasterResponse> RemoveCategoryAsync(
        Guid masterId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            masterId,
            (master, now) => master.RemoveCategory(categoryId, now),
            "Category removed from master {MasterId}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<MasterResponse> SubmitForVerificationAsync(
        Guid masterId,
        CancellationToken cancellationToken)
    {
        return ChangeAsync(
            masterId,
            (master, now) => master.SubmitForVerification(now),
            "Master {MasterId} submitted for verification",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MasterFileRemoval> RemoveAvatarAsync(
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var master = await masters.GetRequiredAsync(masterId, cancellationToken);
        var avatar = master.Avatar ?? throw new InvalidAvatarException("The profile has no avatar.");
        var now = timeProvider.GetUtcNow();

        master.RemoveAvatar(now);
        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Avatar of master {MasterId} removed", masterId);

        return new MasterFileRemoval(
            master.ToResponse(now),
            new StoredFileResponse(_storage.AvatarsBucket, avatar.StorageKey));
    }

    /// <inheritdoc />
    public async Task<MasterFileRemoval> RemoveDocumentAsync(
        Guid masterId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var master = await masters.GetRequiredAsync(masterId, cancellationToken);
        var document = master.Documents.FirstOrDefault(uploaded => uploaded.Id == documentId)
            ?? throw new DocumentNotFoundException(documentId);
        var now = timeProvider.GetUtcNow();

        master.RemoveDocument(documentId, now);
        await masters.UpdateAsync(master, cancellationToken);

        logger.LogInformation("Document {DocumentId} of master {MasterId} removed", documentId, masterId);

        return new MasterFileRemoval(
            master.ToResponse(now),
            new StoredFileResponse(_storage.DocumentsBucket, document.StorageKey));
    }

    private async Task<MasterResponse> ChangeAsync(
        Guid masterId,
        Action<Master, DateTimeOffset> change,
        string completedMessage,
        CancellationToken cancellationToken)
    {
        var master = await masters.GetRequiredAsync(masterId, cancellationToken);
        var now = timeProvider.GetUtcNow();

        change(master, now);
        await masters.UpdateAsync(master, cancellationToken);

#pragma warning disable CA2254 // The template is a constant chosen by the caller.
        logger.LogInformation(completedMessage, masterId);
#pragma warning restore CA2254

        return master.ToResponse(now);
    }

    private async Task<List<Category>> LoadCategoriesAsync(
        IReadOnlyCollection<Guid> categoryIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = categoryIds.Distinct().ToList();

        var found = await categories
            .AsQueryable()
            .Where(category => distinctIds.Contains(category.Id))
            .ToListAsync(cancellationToken);

        var missingId = distinctIds.Except(found.Select(category => category.Id)).FirstOrDefault();

        if (missingId != Guid.Empty)
        {
            throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Category '{0}' was not found.",
                missingId));
        }

        return found;
    }
}