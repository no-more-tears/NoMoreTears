using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetNotifications;

/// <summary>
///     Returns a page of notifications of the recipient, newest first.
/// </summary>
public sealed record GetNotificationsQuery(
    Guid RecipientId,
    bool UnreadOnly,
    string? Category,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<NotificationResponse>>, IPagedQuery;
