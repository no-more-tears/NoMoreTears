namespace NoMoreTears.Shared.Common.Authorization;

/// <summary>
///     Defines the roles available across the platform.
/// </summary>
public enum UserRole
{
    PlatformAdministrator,
    PlatformSupport,

    /// <summary>
    ///     A verified master. Identity grants the role when Masters verifies the profile of the user.
    /// </summary>
    Master,

    /// <summary>
    ///     Every registered user. A client can create a master profile and submit it for verification.
    /// </summary>
    Client,
}