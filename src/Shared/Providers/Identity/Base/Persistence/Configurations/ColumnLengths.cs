namespace NoMoreTears.Shared.Providers.Identity.Persistence.Configurations;

/// <summary>
///     Maximum lengths of string columns, shared across entity configurations.
/// </summary>
internal static class ColumnLengths
{
    /// <summary>
    ///     RFC 5321 maximum length of an email address.
    /// </summary>
    public const int Email = 320;

    public const int Username = 64;

    /// <summary>
    ///     Fits a BCrypt hash with room for future algorithms.
    /// </summary>
    public const int PasswordHash = 128;

    /// <summary>
    ///     Fits a base64url-encoded SHA-256 digest.
    /// </summary>
    public const int Hash = 64;

    /// <summary>
    ///     Fits an IPv6 address with an embedded IPv4 suffix.
    /// </summary>
    public const int IpAddress = 45;

    public const int UserAgent = 512;

    public const int DeviceId = 128;

    /// <summary>
    ///     Enumerations stored by name.
    /// </summary>
    public const int EnumName = 32;
}