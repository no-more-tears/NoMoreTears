using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Identity.Contracts.Responses;

namespace NoMoreTears.Modules.Identity.Applications.Services.Results;

/// <summary>
///     Result of a sign-in; <see cref="MfaCode"/> is set when a one-time code must be delivered.
/// </summary>
public sealed record LoginOutcome(
    LoginResponse Response,
    MfaCodeIssuedEvent? MfaCode);
