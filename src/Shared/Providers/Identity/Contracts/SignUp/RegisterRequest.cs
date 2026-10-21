using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;

/// <summary>
///     Data required to register a new user.
/// </summary>
/// <param name="Email">Email address, must be unique across all users.</param>
/// <param name="Password">Plain-text password, it is hashed before being persisted.</param>
/// <param name="Username">Optional unique username; when omitted the email address is used.</param>
/// <param name="Role">Initial role assigned to the user.</param>
public sealed record RegisterRequest(
    string Email,
    string Password,
    string? Username = null,
    UserRole Role = UserRole.Client);
