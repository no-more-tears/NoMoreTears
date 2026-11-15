using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Mappings;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Transitions that happen when a deadline passes. Every batch takes a transaction-level advisory lock,
///     so several instances of the background jobs never process the same operation at the same time.
/// </summary>
public sealed class DeadlineService(
    IRepository<Request, Guid> requests,
    TimeProvider timeProvider,
    ILogger<DeadlineService> logger) : IDeadlineService
{
    private const string AdvisoryLockSql = "SELECT pg_advisory_xact_lock({0})";
    private const long ExpirationLockKey = 0x5245_5155_4558_5052;
    private const long CompletionLockKey = 0x5245_5155_434F_4D50;

    /// <inheritdoc />
    public async Task<IReadOnlyList<RequestResponse>> ExpireOverdueAsync(
        int batchSize,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

        await requests.ExecuteRawSqlAsync(AdvisoryLockSql, cancellationToken, ExpirationLockKey);

        var now = timeProvider.GetUtcNow();
        var overdue = await requests
            .AsQueryable()
            .Where(request => request.Status == RequestStatus.Published && request.AcceptanceDeadline <= now)
            .OrderBy(request => request.AcceptanceDeadline)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (overdue.Count == 0)
        {
            return [];
        }

        foreach (var request in overdue)
        {
            request.Expire(now);
        }

        await requests.UpdateManyAsync([.. overdue], cancellationToken);

        logger.LogInformation("{Count} requests expired", overdue.Count);
        return [.. overdue.Select(request => request.ToResponse())];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RequestResponse>> CompleteOverdueAsync(
        int batchSize,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

        await requests.ExecuteRawSqlAsync(AdvisoryLockSql, cancellationToken, CompletionLockKey);

        var now = timeProvider.GetUtcNow();
        var overdue = await requests
            .AsQueryable()
            .Where(request => request.Status == RequestStatus.AwaitingConfirmation && request.ConfirmationDeadline <= now)
            .OrderBy(request => request.ConfirmationDeadline)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (overdue.Count == 0)
        {
            return [];
        }

        foreach (var request in overdue)
        {
            request.CompleteAutomatically(now);
        }

        await requests.UpdateManyAsync([.. overdue], cancellationToken);

        logger.LogInformation("{Count} requests completed automatically", overdue.Count);
        return [.. overdue.Select(request => request.ToResponse())];
    }
}