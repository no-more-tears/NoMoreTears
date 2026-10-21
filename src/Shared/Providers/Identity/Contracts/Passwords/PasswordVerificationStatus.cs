namespace NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;

/// <summary>
///     Result of comparing a plain-text password against a stored hash.
/// </summary>
public enum PasswordVerificationStatus
{
    /// <summary>
    ///     The password does not match the hash.
    /// </summary>
    Failed,

    /// <summary>
    ///     The password matches the hash.
    /// </summary>
    Success,

    /// <summary>
    ///     The password matches, but the hash was produced with outdated parameters
    ///     and should be recomputed.
    /// </summary>
    SuccessRehashNeeded
}
