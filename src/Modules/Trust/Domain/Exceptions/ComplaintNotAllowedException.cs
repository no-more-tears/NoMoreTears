using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a complaint is submitted in violation.
/// </summary>
public sealed class ComplaintNotAllowedException(string message)
    : DomainException(message);