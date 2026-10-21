using UAParser;

namespace NoMoreTears.Shared.Common.Authorization;

/// <summary>
///     Represents the current authenticated user's session context.
///     Default implementation of <see cref="ICurrentUserSession"/>.
/// </summary>
public class CurrentUserSession : ICurrentUserSession
{
    /// <inheritdoc />
    public Guid? UserId { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<UserRole>? Roles { get; }

    /// <inheritdoc />
    public Guid? SessionId { get; }

    /// <inheritdoc />
    public string? IpAddress { get; }

    /// <inheritdoc />
    public string? UserAgent { get; }

    /// <inheritdoc />
    public string? DeviceName { get; }

    /// <inheritdoc />
    public string? OsName { get; }

    /// <inheritdoc />
    public string? OsVersion { get; }

    /// <inheritdoc />
    public string? BrowserName { get; }

    /// <inheritdoc />
    public string? BrowserVersion { get; }

    /// <inheritdoc />
    public bool IsAuthenticated => UserId.HasValue;

    /// <inheritdoc />
    public bool? IsPlatformLevel => Roles?.Any(r =>
        r is UserRole.PlatformAdministrator or UserRole.PlatformSupport);

    /// <summary>
    ///     Initializes an unauthenticated session.
    /// </summary>
    public CurrentUserSession()
    {
    }

    /// <summary>
    ///     Initializes an authenticated session with the provided JWT-resolved values.
    /// </summary>
    public CurrentUserSession(
        Guid? userId,
        IReadOnlyCollection<UserRole>? roles,
        Guid? sessionId,
        string? ipAddress,
        string? userAgent)
    {
        UserId = userId;
        Roles = roles;
        SessionId = sessionId;
        IpAddress = ipAddress;
        UserAgent = userAgent;

        if (userAgent is not null)
        {
            var parsedUserAgent = Parser.GetDefault().Parse(userAgent);

            DeviceName = NormalizeUserAgentSegment(parsedUserAgent.Device.Family);
            OsName = NormalizeUserAgentSegment(parsedUserAgent.OS.Family);
            OsVersion = BuildVersion(parsedUserAgent.OS.Major, parsedUserAgent.OS.Minor);
            BrowserName = NormalizeUserAgentSegment(parsedUserAgent.UA.Family);
            BrowserVersion = BuildVersion(parsedUserAgent.UA.Major, parsedUserAgent.UA.Minor);
        }
    }

    private static string? NormalizeUserAgentSegment(string value) =>
        string.IsNullOrWhiteSpace(value) || value == "Other" ? null : value;

    private static string? BuildVersion(string? major, string? minor) =>
        major is null ? null : minor is null ? major : $"{major}.{minor}";
}