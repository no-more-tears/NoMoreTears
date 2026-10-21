using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A single-use token for email confirmation or password reset.
/// </summary>
internal sealed class UserActionTokenEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public UserActionPurpose Purpose { get; set; }

    public required string TokenHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }
}
