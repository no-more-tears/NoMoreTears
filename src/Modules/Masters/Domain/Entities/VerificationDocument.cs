using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.Entities;

/// <summary>
///     Identity or qualification document kept in restricted object storage.
/// </summary>
public sealed class VerificationDocument
{
    /// <summary>
    ///     Unique identifier of the document.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Kind of the document.
    /// </summary>
    public DocumentKind Kind { get; }

    /// <summary>
    ///     Category confirmed by a qualification document.
    /// </summary>
    public Guid? CategoryId { get; }

    /// <summary>
    ///     Key of the object in object storage.
    /// </summary>
    public string StorageKey { get; }

    /// <summary>
    ///     MIME type of the document.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    ///     Size of the document in bytes.
    /// </summary>
    public long SizeInBytes { get; }

    /// <summary>
    ///     Moment of upload.
    /// </summary>
    public DateTimeOffset UploadedAt { get; }

    /// <summary>
    ///     Returns whether the document confirms the qualification in the given category.
    /// </summary>
    public bool ConfirmsQualificationIn(Guid categoryId) => Kind is DocumentKind.Qualification && CategoryId == categoryId;

    private VerificationDocument(
        Guid id,
        DocumentKind kind,
        Guid? categoryId,
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAt)
    {
        Id = id;
        Kind = kind;
        CategoryId = categoryId;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        UploadedAt = uploadedAt;
    }

    /// <summary>
    ///     Creates a document. Only the <see cref="Aggregates.Master"/> aggregate uploads documents.
    /// </summary>
    internal static VerificationDocument Create(
        DocumentKind kind,
        Guid? categoryId,
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAt)
    {
        var normalizedKey = storageKey?.Trim() ?? string.Empty;
        var normalizedContentType = contentType?.Trim() ?? string.Empty;

        if ((kind is DocumentKind.Qualification) != categoryId.HasValue)
        {
            throw new InvalidDocumentException("Only a qualification document refers to a category.");
        }

        if (normalizedKey.Length == 0 || normalizedKey.Length > MasterRules.MaxStorageKeyLength)
        {
            throw new InvalidDocumentException(
                $"Storage key must contain from 1 to {MasterRules.MaxStorageKeyLength} characters.");
        }

        if (!MasterRules.AllowedDocumentContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidDocumentException($"Content type '{normalizedContentType}' is not allowed.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > MasterRules.MaxDocumentSizeInBytes)
        {
            throw new InvalidDocumentException(
                $"Document size must be from 1 to {MasterRules.MaxDocumentSizeInBytes} bytes.");
        }

        return new VerificationDocument(
            Guid.CreateVersion7(uploadedAt),
            kind,
            categoryId,
            normalizedKey,
            normalizedContentType,
            sizeInBytes,
            uploadedAt);
    }
}