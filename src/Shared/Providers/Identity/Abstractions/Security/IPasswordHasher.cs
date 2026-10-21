using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Security;

/// <summary>
///     Hashes passwords and verifies them against stored hashes.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    ///     Produces a salted hash of the password that is safe to persist.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    ///     Compares the password against the stored hash.
    /// </summary>
    /// <remarks>
    ///     Returns <see cref="PasswordVerificationStatus.SuccessRehashNeeded"/> when the hash
    ///     matches but was produced with weaker parameters than currently configured.
    /// </remarks>
    PasswordVerificationStatus VerifyHashedPassword(
        string password,
        string hashedPassword);
}