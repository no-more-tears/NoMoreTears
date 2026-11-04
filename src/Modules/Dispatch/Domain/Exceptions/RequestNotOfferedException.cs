using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master acts on a request that is not in their feed.
/// </summary>
public sealed class RequestNotOfferedException(Guid masterId)
    : DomainException($"The request is not offered to master '{masterId}'.");