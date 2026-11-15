using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a client with an active publication block tries to publish a request.
/// </summary>
public sealed class PublicationBlockedException(DateTimeOffset? blockedUntil)
    : DomainException(blockedUntil is null
        ? "Publication of requests is blocked until the restriction is lifted."
        : $"Publication of requests is blocked until {blockedUntil:O}.");