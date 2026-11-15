using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a master without feed access or without the verified category of the request tries to accept it.
/// </summary>
public sealed class MasterNotEligibleException()
    : DomainException("The master has no access to requests of this category.");