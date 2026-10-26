namespace NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

/// <summary>
///     Claim types emitted in access tokens issued by the identity provider.
/// </summary>
/// <remarks>
///     Short JWT names are used as-is; inbound claim type mapping has to be disabled
///     on the consuming side so the names stay stable.
/// </remarks>
public static class IdentityClaimTypes
{
    /// <summary>
    ///     Identifier of the user (<c>sub</c>).
    /// </summary>
    public const string Subject = "sub";

    /// <summary>
    ///     Email address of the user (<c>email</c>).
    /// </summary>
    public const string Email = "email";

    /// <summary>
    ///     Username of the user (<c>name</c>).
    /// </summary>
    public const string Name = "name";

    /// <summary>
    ///     Role of the user (<c>role</c>); emitted once per assigned role.
    /// </summary>
    public const string Role = "role";

    /// <summary>
    ///     Identifier of the session the token belongs to (<c>sid</c>).
    /// </summary>
    public const string SessionId = "sid";

    /// <summary>
    ///     Unique identifier of the token (<c>jti</c>).
    /// </summary>
    public const string TokenId = "jti";

    /// <summary>
    ///     Methods used to authenticate the session (<c>amr</c>); see <see cref="AuthenticationMethods"/>.
    /// </summary>
    public const string AuthenticationMethods = "amr";
}