using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a request is cancelled after acceptance without a reason.
/// </summary>
public sealed class CancellationReasonRequiredException()
    : DomainException("Cancelling a request after acceptance requires a reason.");