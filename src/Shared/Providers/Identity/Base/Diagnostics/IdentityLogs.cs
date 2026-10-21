using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

namespace NoMoreTears.Shared.Providers.Identity.Diagnostics;

/// <summary>
///     Source-generated log messages of the identity provider.
/// </summary>
internal static partial class IdentityLogs
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information,
        Message = "User {UserId} registered.")]
    public static partial void UserRegistered(this ILogger logger, Guid userId);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning,
        Message = "Account {UserId} locked until {LockoutEndAt} after too many failed sign-in attempts.")]
    public static partial void AccountLocked(this ILogger logger, Guid userId, DateTimeOffset lockoutEndAt);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Warning,
        Message = "Refresh token reuse detected for user {UserId}; session {SessionId} revoked.")]
    public static partial void RefreshTokenReuseDetected(this ILogger logger, Guid userId, Guid sessionId);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information,
        Message = "Revoked {Count} session(s) of user {UserId}. Reason={Reason}")]
    public static partial void SessionsRevoked(this ILogger logger, Guid userId, SessionRevocationReason reason, int count);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Information,
        Message = "Identity maintenance removed {Count} expired row(s).")]
    public static partial void MaintenanceCompleted(this ILogger logger, int count);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Error,
        Message = "Identity maintenance failed.")]
    public static partial void MaintenanceFailed(this ILogger logger, Exception exception);
}