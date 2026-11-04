using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master tries to accept a request that another master has already accepted.
/// </summary>
public sealed class RequestAlreadyAcceptedException()
    : DomainException("The request has already been accepted.");