using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Media file of evidence kept in object storage.
/// </summary>
public sealed record EvidenceFile
{
    /// <summary>
    ///     Key of the object in object storage.
    /// </summary>
    public string StorageKey { get; }

    /// <summary>
    ///     MIME type of the file.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    ///     Size of the file in bytes.
    /// </summary>
    public long SizeInBytes { get; }

    private EvidenceFile(
        string storageKey,
        string contentType,
        long sizeInBytes)
    {
        StorageKey = storageKey;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
    }

    /// <summary>
    ///     Creates an evidence file.
    /// </summary>
    public static EvidenceFile Of(
        string storageKey,
        string contentType,
        long sizeInBytes)
    {
        var normalizedKey = storageKey?.Trim() ?? string.Empty;
        var normalizedContentType = contentType?.Trim() ?? string.Empty;

        if (normalizedKey.Length == 0 || normalizedKey.Length > TrustRules.MaxStorageKeyLength)
        {
            throw new InvalidEvidenceFileException(
                $"Storage key must contain from 1 to {TrustRules.MaxStorageKeyLength} characters.");
        }

        if (!TrustRules.AllowedEvidenceContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidEvidenceFileException($"Content type '{normalizedContentType}' is not allowed.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > TrustRules.MaxEvidenceFileSizeInBytes)
        {
            throw new InvalidEvidenceFileException(
                $"Evidence file size must be from 1 to {TrustRules.MaxEvidenceFileSizeInBytes} bytes.");
        }

        return new EvidenceFile(normalizedKey, normalizedContentType, sizeInBytes);
    }
}