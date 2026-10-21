using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

/// <summary>
///     Data required to exchange a refresh token for a new token pair.
/// </summary>
/// <param name="RefreshToken">Refresh token issued with the previous token pair.</param>
/// <param name="Client">Client that performs the refresh.</param>
public sealed record RefreshRequest(
    string RefreshToken,
    ClientContext Client);
