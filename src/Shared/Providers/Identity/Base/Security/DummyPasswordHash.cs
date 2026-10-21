using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;

namespace NoMoreTears.Shared.Providers.Identity.Security;

/// <summary>
///     A hash of a random password computed once per process. It is verified instead of a real hash
///     when the user does not exist, so response timing does not reveal whether an account exists.
/// </summary>
internal sealed class DummyPasswordHash(IPasswordHasher passwordHasher)
{
    private const int PasswordSizeInBytes = 32;

    private readonly Lazy<string> _value = new(
        () => passwordHasher.HashPassword(TokenSecret.Generate(PasswordSizeInBytes)),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    ///     The precomputed hash.
    /// </summary>
    public string Value => _value.Value;
}