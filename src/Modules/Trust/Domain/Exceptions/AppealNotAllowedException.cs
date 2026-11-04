using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an appeal is submitted in violation.
/// </summary>
public sealed class AppealNotAllowedException(string message)
    : DomainException(message);