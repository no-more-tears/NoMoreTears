namespace NoMoreTears.Shared.Common.Authorization;

/// <summary>
///     Represents the current authenticated user's session context.
/// </summary>
public interface ICurrentUserSession
{
    /// <summary>
    ///     Unique identifier of the authenticated user.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    ///     Roles assigned to the current user.
    /// </summary>
    IReadOnlyCollection<UserRole>? Roles { get; }

    /// <summary>
    ///     Unique identifier of the current session.</summary>
    Guid? SessionId { get; }

    /// <summary>
    ///     IP address of the client that initiated the request.
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    ///     Raw value of the <c>User-Agent</c> http header.
    /// </summary>
    string? UserAgent { get; }

    /// <summary>
    ///     Device name parsed from the <c>User-Agent</c> header.
    /// </summary>
    string? DeviceName { get; }

    /// <summary>
    ///     Operating system name parsed from the <c>User-Agent</c> header.
    /// </summary>
    string? OsName { get; }

    /// <summary>
    ///     Operating system version parsed from the <c>User-Agent</c> header.
    /// </summary>
    string? OsVersion { get; }

    /// <summary>
    ///     Browser name parsed from the <c>User-Agent</c> header.
    /// </summary>
    string? BrowserName { get; }

    /// <summary>
    ///     Browser version parsed from the <c>User-Agent</c> header.
    /// </summary>
    string? BrowserVersion { get; }

    /// <summary>
    ///     Indicates whether the current session context belongs to an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Indicates whether the current user holds any platform-level role.
    /// </summary>
    bool? IsPlatformLevel { get; }
}