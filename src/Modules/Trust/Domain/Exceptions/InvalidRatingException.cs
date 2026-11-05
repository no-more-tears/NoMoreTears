using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a rating is outside the 1–5 scale.
/// </summary>
public sealed class InvalidRatingException(int value)
    : DomainException($"Rating must be from 1 to 5, but was {value}.");