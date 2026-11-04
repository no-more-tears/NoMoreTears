using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when the content of a request is changed after publication.
/// </summary>
public sealed class RequestContentLockedException(RequestStatus status)
    : DomainException($"Request content can be changed only while it is a draft, current status is '{status}'.");