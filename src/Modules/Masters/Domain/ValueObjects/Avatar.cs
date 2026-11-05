using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Profile photo of a master kept in object storage.
/// </summary>
public sealed record Avatar
{
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

    private Avatar(
        string storageKey,
        string contentType,
        long sizeInBytes)
    {
        StorageKey = storageKey;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
    }

    /// <summary>
    ///     Creates an avatar.
    /// </summary>
    public static Avatar Of(
        string storageKey,
        string contentType,
        long sizeInBytes)
    {
        var normalizedKey = storageKey?.Trim() ?? string.Empty;
        var normalizedContentType = contentType?.Trim() ?? string.Empty;

        if (normalizedKey.Length == 0 || normalizedKey.Length > MasterRules.MaxStorageKeyLength)
        {
            throw new InvalidAvatarException(
                $"Storage key must contain from 1 to {MasterRules.MaxStorageKeyLength} characters.");
        }

        if (!MasterRules.AllowedAvatarContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidAvatarException(
                $"Content type '{normalizedContentType}' is not allowed, only photos are accepted.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > MasterRules.MaxAvatarSizeInBytes)
        {
            throw new InvalidAvatarException(
                $"Avatar size must be from 1 to {MasterRules.MaxAvatarSizeInBytes} bytes.");
        }

        return new Avatar(normalizedKey, normalizedContentType, sizeInBytes);
    }
}