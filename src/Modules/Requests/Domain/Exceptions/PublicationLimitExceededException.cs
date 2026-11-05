using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when publishing would exceed the client limit of simultaneously published requests.
/// </summary>
public sealed class PublicationLimitExceededException(int limit)
    : DomainException($"A client can have at most {limit} published requests at the same time.");