using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.Contracts.Responses;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string Username,
    bool EmailConfirmed,
    bool IsActive,
    bool MfaEnabled,
    IReadOnlyCollection<UserRole> Roles,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);