using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master suspected of automation accepts a request without step-up verification.
/// </summary>
public sealed class StepUpVerificationRequiredException()
    : DomainException("Additional verification with MFA is required before accepting requests.");