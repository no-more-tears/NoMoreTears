namespace NoMoreTears.Shared.Providers.Identity.Contracts.Common;

/// <summary>
///     Describes the client that initiated an identity operation.
/// </summary>
/// <remarks>
///     The values are stored with the session for auditing purposes and are used to bind
///     MFA challenges to the client that started them. Every value is optional,
///     the host supplies whatever it is able to resolve from the incoming request.
/// </remarks>
/// <param name="IpAddress">IP address of the client.</param>
/// <param name="UserAgent">Raw value of the <c>User-Agent</c> http header.</param>
/// <param name="DeviceId">Stable device identifier assigned by the host application.</param>
public sealed record ClientContext(
    string? IpAddress,
    string? UserAgent,
    string? DeviceId)
{
    /// <summary>
    ///     A client context without any resolved values.
    /// </summary>
    public static ClientContext Unknown { get; } = new(null, null, null);
}
