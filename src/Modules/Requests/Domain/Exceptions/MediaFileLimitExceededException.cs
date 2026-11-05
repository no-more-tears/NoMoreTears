using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when attaching a media file would exceed the per-request limit.
/// </summary>
public sealed class MediaFileLimitExceededException(int limit)
    : DomainException($"A request can contain at most {limit} media files.");