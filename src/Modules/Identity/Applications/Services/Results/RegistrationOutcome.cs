using NoMoreTears.Modules.Identity.Contracts.Events;

namespace NoMoreTears.Modules.Identity.Applications.Services.Results;

/// <summary>
///     Registered user with the confirmation email to request.
/// </summary>
public sealed record RegistrationOutcome(
    Guid UserId,
    EmailConfirmationRequestedEvent EmailConfirmation);
