using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace NoMoreTears.Shared.Providers.Identity.Security;

/// <summary>
///     Generation and hashing of opaque secrets (refresh tokens, challenge tokens, action tokens).
/// </summary>
/// <remarks>
///     Secrets are only ever persisted as SHA-256 digests, so a leaked table cannot be replayed.
/// </remarks>
internal static class TokenSecret
{
    private const string Digits = "0123456789";

    /// <summary>
    ///     Generates a cryptographically random, base64url-encoded secret.
    /// </summary>
    public static string Generate(int sizeInBytes) =>
        Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(sizeInBytes));

    /// <summary>
    ///     Generates a cryptographically random numeric code of the given length.
    /// </summary>
    public static string GenerateNumericCode(int length) =>
        RandomNumberGenerator.GetString(Digits, length);

    /// <summary>
    ///     Computes the base64url-encoded SHA-256 digest of the value.
    /// </summary>
    public static string Hash(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        return Base64Url.EncodeToString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    /// <summary>
    ///     Compares two digests without leaking the position of the first difference.
    /// </summary>
    public static bool FixedTimeEquals(string left, string right) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(left),
            Encoding.UTF8.GetBytes(right));
}