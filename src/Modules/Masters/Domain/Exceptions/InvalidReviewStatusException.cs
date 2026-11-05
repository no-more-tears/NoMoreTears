using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a review decision is made on an item that is not awaiting review.
/// </summary>
public sealed class InvalidReviewStatusException(ReviewStatus status, string action)
    : DomainException($"Cannot {action} an item with review status '{status}'.");