using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

/// <summary>
///     Credentials presented to sign in.
/// </summary>
/// <param name="Identifier">Email address or username.</param>
/// <param name="Password">Plain-text password.</param>
/// <param name="Client">Client that initiated the sign-in.</param>
public sealed record LoginRequest(
    string Identifier,
    string Password,
    ClientContext Client);
