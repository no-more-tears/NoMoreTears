using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Configuration;
using NoMoreTears.Modules.Notification.Applications.Services.Mappings;
using NoMoreTears.Modules.Notification.Applications.Services.Notifications;
using NoMoreTears.Modules.Notification.Applications.Services.Results;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Notification.Contracts.Responses;
using NoMoreTears.Modules.Notification.Domain.Enums;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Common.Pagination;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;
using NotificationModel = NoMoreTears.Modules.Notification.Domain.Aggregates.Notification;

namespace NoMoreTears.Modules.Notification.Applications.Services;

/// <summary>
///     In-app notifications: storage, reading and real-time delivery.
/// </summary>
public sealed class NotificationService(
    IRepository<NotificationModel, Guid> notifications,
    IRealtimeNotifier realtimeNotifier,
    IOptions<InAppNotificationOptions> options,
    TimeProvider timeProvider,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly InAppNotificationOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<NotificationResponse?> CreateAsync(
        Guid recipientId,
        Guid sourceEventId,
        string type,
        IReadOnlyDictionary<string, string> data,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken)
    {
        var exists = await notifications
            .AsQueryable()
            .IgnoreQueryFilters()
            .AnyAsync(
                found => found.SourceEventId == sourceEventId && found.RecipientId == recipientId,
                cancellationToken);

        if (exists)
        {
            return null;
        }

        var definition = NotificationCatalog.Get(type);
        var text = NotificationTextStore.Render(type, _options.Culture, data);

        var notification = NotificationModel.Create(
            recipientId,
            sourceEventId,
            type,
            definition.Category,
            definition.Severity,
            text.Title,
            text.Body,
            data,
            definition.Link,
            occurredAt,
            timeProvider.GetUtcNow());

        await notifications.CreateAsync(notification, cancellationToken);

        logger.LogInformation("Notification {NotificationId} ({Type}) created for user {RecipientId}", notification.Id, type, recipientId);
        return notification.ToResponse();
    }

    /// <inheritdoc />
    public async Task<PagedResponse<NotificationResponse>> GetPageAsync(
        Guid recipientId,
        bool unreadOnly,
        string? category,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = notifications
            .AsQueryable()
            .AsNoTracking()
            .Where(notification => notification.RecipientId == recipientId);

        if (unreadOnly)
        {
            query = query.Where(notification => notification.ReadAt == null);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!Enum.TryParse<NotificationCategory>(category, ignoreCase: true, out var parsedCategory))
            {
                throw new BadRequestException($"Notification category '{category}' is unknown.");
            }

            query = query.Where(notification => notification.Category == parsedCategory);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var page = await query
            .OrderByDescending(notification => notification.CreatedAt)
            .ThenByDescending(notification => notification.Id)
            .Skip(PagedResponse<NotificationResponse>.Offset(pageNumber, pageSize))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<NotificationResponse>(
            [.. page.Select(notification => notification.ToResponse())],
            pageNumber,
            pageSize,
            totalCount);
    }

    /// <inheritdoc />
    public async Task<UnreadCountResponse> GetUnreadCountAsync(
        Guid recipientId,
        CancellationToken cancellationToken) =>
        new(await CountUnreadAsync(recipientId, cancellationToken));

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationResponse>> GetUndeliveredAsync(
        Guid recipientId,
        CancellationToken cancellationToken)
    {
        var undelivered = await notifications
            .AsQueryable()
            .AsNoTracking()
            .Where(notification => notification.RecipientId == recipientId && notification.DeliveredAt == null)
            .OrderBy(notification => notification.CreatedAt)
            .Take(_options.UndeliveredBatchSize)
            .ToListAsync(cancellationToken);

        return [.. undelivered.Select(notification => notification.ToResponse())];
    }

    /// <inheritdoc />
    public async Task<NotificationReadResult> MarkAsReadAsync(
        Guid recipientId,
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var notification = await GetOwnAsync(recipientId, notificationId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var wasRead = notification.IsRead;

        notification.MarkAsRead(now);
        await notifications.UpdateAsync(notification, cancellationToken);

        var unreadCount = await CountUnreadAsync(recipientId, cancellationToken);

        logger.LogInformation("Notification {NotificationId} read by user {RecipientId}", notificationId, recipientId);

        return new NotificationReadResult(
            notification.ToResponse(),
            new NotificationsChangedResponse(NotificationChanges.Read, [notificationId], wasRead ? 0 : 1, unreadCount, now));
    }

    /// <inheritdoc />
    public async Task<NotificationsChangedResponse> MarkManyAsReadAsync(
        Guid recipientId,
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notificationIds);

        var ids = notificationIds.Distinct().ToList();
        var now = timeProvider.GetUtcNow();

        var unread = await notifications
            .AsQueryable()
            .Where(notification => notification.RecipientId == recipientId
                && ids.Contains(notification.Id)
                && notification.ReadAt == null)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
        {
            notification.MarkAsRead(now);
        }

        await notifications.UpdateManyAsync([.. unread], cancellationToken);

        var unreadCount = await CountUnreadAsync(recipientId, cancellationToken);

        logger.LogInformation("{Count} notifications read by user {RecipientId}", unread.Count, recipientId);

        return new NotificationsChangedResponse(
            NotificationChanges.Read,
            [.. unread.Select(notification => notification.Id)],
            unread.Count,
            unreadCount,
            now);
    }

    /// <inheritdoc />
    public async Task<NotificationsChangedResponse> MarkAllAsReadAsync(
        Guid recipientId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        // A bulk update: a user may have thousands of unread notifications, loading them all would be wasteful.
        var affected = await notifications
            .Where(notification => notification.RecipientId == recipientId && notification.ReadAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(notification => notification.ReadAt, now)
                    .SetProperty(notification => notification.DeliveredAt, notification => notification.DeliveredAt ?? now)
                    .SetProperty(notification => notification.LastUpdatedAt, now),
                cancellationToken);

        logger.LogInformation("All {Count} unread notifications read by user {RecipientId}", affected, recipientId);
        return new NotificationsChangedResponse(NotificationChanges.ReadAll, [], affected, 0, now);
    }

    /// <inheritdoc />
    public async Task<NotificationsChangedResponse> DeleteAsync(
        Guid recipientId,
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var notification = await GetOwnAsync(recipientId, notificationId, cancellationToken);
        var now = timeProvider.GetUtcNow();

        notification.Delete(now);
        await notifications.UpdateAsync(notification, cancellationToken);

        var unreadCount = await CountUnreadAsync(recipientId, cancellationToken);

        logger.LogInformation("Notification {NotificationId} deleted by user {RecipientId}", notificationId, recipientId);
        return new NotificationsChangedResponse(NotificationChanges.Deleted, [notificationId], 1, unreadCount, now);
    }

    /// <inheritdoc />
    public async Task<int> AcknowledgeAsync(
        Guid recipientId,
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notificationIds);

        var ids = notificationIds.Distinct().ToList();
        var now = timeProvider.GetUtcNow();

        var undelivered = await notifications
            .AsQueryable()
            .Where(notification => notification.RecipientId == recipientId
                && ids.Contains(notification.Id)
                && notification.DeliveredAt == null)
            .ToListAsync(cancellationToken);

        foreach (var notification in undelivered)
        {
            notification.MarkAsDelivered(now);
        }

        await notifications.UpdateManyAsync([.. undelivered], cancellationToken);

        logger.LogInformation("{Count} notifications delivered to user {RecipientId}", undelivered.Count, recipientId);
        return undelivered.Count;
    }

    /// <inheritdoc />
    public async Task PushAsync(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var notification = await notifications
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(found => found.Id == notificationId, cancellationToken);

        // Deleted before it was pushed: nothing to deliver.
        if (notification is null)
        {
            return;
        }

        var unreadCount = await CountUnreadAsync(notification.RecipientId, cancellationToken);

        await realtimeNotifier.SendNotificationAsync(notification.RecipientId, notification.ToResponse(), cancellationToken);
        await realtimeNotifier.SendUnreadCountAsync(notification.RecipientId, new UnreadCountResponse(unreadCount), cancellationToken);

        logger.LogInformation("Notification {NotificationId} pushed to user {RecipientId}", notificationId, notification.RecipientId);
    }

    /// <inheritdoc />
    public async Task PushChangeAsync(
        Guid recipientId,
        NotificationsChangedResponse change,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(change);

        await realtimeNotifier.SendNotificationsChangedAsync(recipientId, change, cancellationToken);
        await realtimeNotifier.SendUnreadCountAsync(recipientId, new UnreadCountResponse(change.UnreadCount), cancellationToken);
    }

    private Task<long> CountUnreadAsync(
        Guid recipientId,
        CancellationToken cancellationToken)
    {
        return notifications.CountAsync(
            notification => notification.RecipientId == recipientId && notification.ReadAt == null,
            cancellationToken);
    }

    private async Task<NotificationModel> GetOwnAsync(
        Guid recipientId,
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        return await notifications
            .AsQueryable()
            .FirstOrDefaultAsync(
                notification => notification.Id == notificationId && notification.RecipientId == recipientId,
                cancellationToken)
            ?? throw new NotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "Notification '{0}' was not found.",
                notificationId));
    }
}