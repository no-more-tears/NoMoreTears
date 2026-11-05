using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current status of a review.
/// </summary>
public sealed class InvalidReviewStatusException(ReviewStatus status, string action)
    : DomainException($"Cannot {action} a review in status '{status}'.");