using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a review is left in violation.
/// </summary>
public sealed class ReviewNotAllowedException(string message)
    : DomainException(message);