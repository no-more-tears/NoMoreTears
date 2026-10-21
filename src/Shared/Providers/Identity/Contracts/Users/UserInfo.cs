using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.Users;

/// <summary>
///     Identity data of a user.
/// </summary>
/// <param name="Id">Identifier of the user.</param>
/// <param name="Email">Email address.</param>
/// <param name="Username">Username.</param>
/// <param name="EmailConfirmed">Indicates whether the email address has been confirmed.</param>
/// <param name="IsActive">Indicates whether the account is allowed to sign in.</param>
/// <param name="MfaEnabled">Indicates whether sign-in requires a second factor.</param>
/// <param name="Roles">Roles assigned to the user.</param>
/// <param name="CreatedAt">Moment the account was created.</param>
/// <param name="LastLoginAt">Moment of the most recent successful sign-in.</param>
public sealed record UserInfo(
    Guid Id,
    string Email,
    string Username,
    bool EmailConfirmed,
    bool IsActive,
    bool MfaEnabled,
    IReadOnlyCollection<UserRole> Roles,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);
