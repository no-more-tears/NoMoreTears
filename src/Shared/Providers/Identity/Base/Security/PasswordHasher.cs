using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

namespace NoMoreTears.Shared.Providers.Identity.Security;

/// <summary>
///     BCrypt password hasher using the enhanced (pre-hashed) scheme,
///     which lifts the 72-byte input limit.
/// </summary>
internal sealed class PasswordHasher(
    IOptions<PasswordOptions> options) : IPasswordHasher
{
    private readonly int _workFactor = options.Value.WorkFactor;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, _workFactor);
    }

    /// <inheritdoc />
    public PasswordVerificationStatus VerifyHashedPassword(
        string password,
        string hashedPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        ArgumentException.ThrowIfNullOrEmpty(hashedPassword);

        bool verified;

        try
        {
            verified = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return PasswordVerificationStatus.Failed;
        }

        if (!verified)
        {
            return PasswordVerificationStatus.Failed;
        }

        return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, _workFactor)
            ? PasswordVerificationStatus.SuccessRehashNeeded
            : PasswordVerificationStatus.Success;
    }
}