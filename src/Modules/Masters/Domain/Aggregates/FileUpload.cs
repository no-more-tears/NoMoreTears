using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Aggregates;

/// <summary>
///     A file sent by a master that waits for the check before it becomes an avatar or a verification document.
///     The file is accepted asynchronously, so the upload keeps its status for the master.
/// </summary>
public sealed class FileUpload : AuditableEntity<Guid>
{
    /// <summary>
    ///     Master who sent the file.
    /// </summary>
    public Guid MasterId { get; private set; }

    /// <summary>
    ///     What the file becomes once it is accepted.
    /// </summary>
    public UploadPurpose Purpose { get; private set; }

    /// <summary>
    ///     Category confirmed by a qualification document.
    /// </summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>
    ///     Original name of the file.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    ///     Declared media type of the file.
    /// </summary>
    public string ContentType { get; private set; }

    /// <summary>
    ///     Size of the file.
    /// </summary>
    public long SizeInBytes { get; private set; }

    /// <summary>
    ///     Key of the file in the temporary storage until the check.
    /// </summary>
    public string StagingKey { get; private set; }

    /// <summary>
    ///     Processing status.
    /// </summary>
    public UploadStatus Status { get; private set; }

    /// <summary>
    ///     Why the file was not accepted.
    /// </summary>
    public Reason? FailureReason { get; private set; }

    /// <summary>
    ///     Verification document created from the file; empty for an avatar.
    /// </summary>
    public Guid? DocumentId { get; private set; }

    /// <summary>
    ///     Moment the processing finished.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>
    ///     Used only by persistence to materialize the upload.
    /// </summary>
    private FileUpload()
    {
        FileName = null!;
        ContentType = null!;
        StagingKey = null!;
    }

    private FileUpload(
        Guid id,
        Guid masterId,
        UploadPurpose purpose,
        Guid? categoryId,
        string fileName,
        string contentType,
        long sizeInBytes,
        string stagingKey)
    {
        Id = id;
        MasterId = masterId;
        Purpose = purpose;
        CategoryId = categoryId;
        FileName = fileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        StagingKey = stagingKey;
        Status = UploadStatus.Pending;
    }

    /// <summary>
    ///     Registers a received file. The declared type and size must fit the purpose,
    ///     the content itself is checked later.
    /// </summary>
    public static FileUpload Create(
        Guid uploadId,
        Guid masterId,
        UploadPurpose purpose,
        Guid? categoryId,
        string fileName,
        string contentType,
        long sizeInBytes,
        string stagingKey,
        DateTimeOffset createdAt)
    {
        if (uploadId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(uploadId));
        }

        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        if ((purpose is UploadPurpose.QualificationDocument) != categoryId.HasValue)
        {
            throw new InvalidFileUploadException("Only a qualification document refers to a category.");
        }

        var normalizedFileName = Path.GetFileName(fileName?.Trim() ?? string.Empty);
        var normalizedContentType = contentType?.Trim() ?? string.Empty;
        var normalizedKey = stagingKey?.Trim() ?? string.Empty;

        if (normalizedFileName.Length == 0 || normalizedFileName.Length > MasterRules.MaxFileNameLength)
        {
            throw new InvalidFileUploadException(
                $"File name must contain from 1 to {MasterRules.MaxFileNameLength} characters.");
        }

        if (normalizedKey.Length == 0 || normalizedKey.Length > MasterRules.MaxStorageKeyLength)
        {
            throw new InvalidFileUploadException(
                $"Storage key must contain from 1 to {MasterRules.MaxStorageKeyLength} characters.");
        }

        var (allowedContentTypes, maxSize) = purpose is UploadPurpose.Avatar
            ? (MasterRules.AllowedAvatarContentTypes, MasterRules.MaxAvatarSizeInBytes)
            : (MasterRules.AllowedDocumentContentTypes, MasterRules.MaxDocumentSizeInBytes);

        if (!allowedContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidFileUploadException($"Content type '{normalizedContentType}' is not allowed.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > maxSize)
        {
            throw new InvalidFileUploadException($"File size must be from 1 to {maxSize} bytes.");
        }

        var upload = new FileUpload(
            uploadId,
            masterId,
            purpose,
            categoryId,
            normalizedFileName,
            normalizedContentType,
            sizeInBytes,
            normalizedKey);

        upload.MarkAsCreated(createdAt);
        return upload;
    }

    /// <summary>
    ///     Marks the file as accepted. A document upload refers to the created document.
    /// </summary>
    public void Complete(
        Guid? documentId,
        DateTimeOffset completedAt)
    {
        EnsurePending("complete");

        if ((Purpose is UploadPurpose.Avatar) == documentId.HasValue)
        {
            throw new InvalidFileUploadException("Only a document upload refers to a document.");
        }

        Status = UploadStatus.Completed;
        DocumentId = documentId;
        ProcessedAt = completedAt;
        MarkAsUpdated(completedAt);
    }

    /// <summary>
    ///     Marks the file as rejected with a reason.
    /// </summary>
    public void Fail(
        Reason reason,
        DateTimeOffset failedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsurePending("fail");

        Status = UploadStatus.Failed;
        FailureReason = reason;
        ProcessedAt = failedAt;
        MarkAsUpdated(failedAt);
    }

    private void EnsurePending(string action)
    {
        if (Status is not UploadStatus.Pending)
        {
            throw new InvalidUploadStatusException(Status, action);
        }
    }
}