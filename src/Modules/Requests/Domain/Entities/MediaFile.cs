using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.Entities;

/// <summary>
///     Photo attached to a request and kept in object storage.
/// </summary>
public sealed class MediaFile
{
    /// <summary>
    ///     Unique identifier of the media file.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Key of the object in object storage.
    /// </summary>
    public string StorageKey { get; }

    /// <summary>
    ///     MIME type of the photo.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    ///     Size of the photo in bytes.
    /// </summary>
    public long SizeInBytes { get; }

    /// <summary>
    ///     Moment the photo was attached.
    /// </summary>
    public DateTimeOffset AttachedAt { get; }

    private MediaFile(
        Guid id,
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset attachedAt)
    {
        Id = id;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        AttachedAt = attachedAt;
    }

    /// <summary>
    ///     Creates a media file.
    ///     Only the <see cref="Aggregates.Request"/> aggregate attaches media files.
    /// </summary>
    internal static MediaFile Create(
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset attachedAt)
    {
        var normalizedKey = storageKey?.Trim() ?? string.Empty;
        var normalizedContentType = contentType?.Trim() ?? string.Empty;

        if (normalizedKey.Length == 0 || normalizedKey.Length > RequestRules.MaxStorageKeyLength)
        {
            throw new InvalidMediaFileException(
                $"Storage key must contain from 1 to {RequestRules.MaxStorageKeyLength} characters.");
        }

        if (!RequestRules.AllowedMediaContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidMediaFileException(
                $"Content type '{normalizedContentType}' is not allowed, only photos are accepted.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > RequestRules.MaxMediaFileSizeInBytes)
        {
            throw new InvalidMediaFileException(
                $"Media file size must be from 1 to {RequestRules.MaxMediaFileSizeInBytes} bytes.");
        }

        return new MediaFile(
            Guid.CreateVersion7(attachedAt),
            normalizedKey,
            normalizedContentType,
            sizeInBytes,
            attachedAt);
    }
}