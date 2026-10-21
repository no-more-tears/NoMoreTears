using NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions;

/// <summary>
///     Combined identity facade with sign-up, sign-in, account, session
///     and user administration capabilities.
/// </summary>
/// <remarks>
///     Consumers that need a narrow dependency should prefer one of the segregated
///     interfaces; the facade is registered for all of them.
///     Authentication failures surface as <c>UnauthorizedException</c>, policy violations
///     as <c>ForbiddenException</c>, duplicates as <c>ConflictException</c> and
///     invalid input as <c>BadRequestException</c> or the standard argument exceptions.
///     Email delivery is intentionally outside of the provider; it only issues tickets.
/// </remarks>
public interface IIdentityProvider
    : ISignUpService, ISignInService, IAccountService, ISessionService, IUserAdministrationService;