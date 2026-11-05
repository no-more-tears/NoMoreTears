using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a media file is not attached to the request.
/// </summary>
public sealed class MediaFileNotFoundException(Guid mediaFileId)
    : DomainException($"Media file '{mediaFileId}' is not attached to the request.");