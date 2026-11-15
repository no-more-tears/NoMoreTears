using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Aggregates;

/// <summary>
///     Photo received for a draft and waiting for the check, or the outcome of the check.
///     The photo is attached to the request only after its content is verified.
/// </summary>
public sealed class MediaUpload : AuditableEntity<Guid>
{
    /// <summary>
    ///     Request the photo is uploaded for.
    /// </summary>
    public Guid RequestId { get; private set; }

    /// <summary>
    ///     Client who uploaded the photo.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    ///     Original file name, without a path.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    ///     Declared media type.
    /// </summary>
    public string ContentType { get; private set; }

    /// <summary>
    ///     Declared size in bytes.
    /// </summary>
    public long SizeInBytes { get; private set; }

    /// <summary>
    ///     Key of the raw file in the temporary bucket.
    /// </summary>
    public string StagingKey { get; private set; }

    /// <summary>
    ///     Processing status.
    /// </summary>
    public UploadStatus Status { get; private set; }

    /// <summary>
    ///     Why the photo was rejected.
    /// </summary>
    public Reason? FailureReason { get; private set; }

    /// <summary>
    ///     Media file of the request created from the accepted photo.
    /// </summary>
    public Guid? MediaFileId { get; private set; }

    /// <summary>
    ///     Moment the check finished.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>
    ///     Used only by persistence to materialize the upload.
    /// </summary>
    private MediaUpload()
    {
        FileName = null!;
        ContentType = null!;
        StagingKey = null!;
    }

    private MediaUpload(
        Guid id,
        Guid requestId,
        Guid clientId,
        string fileName,
        string contentType,
        long sizeInBytes,
        string stagingKey)
    {
        Id = id;
        RequestId = requestId;
        ClientId = clientId;
        FileName = fileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        StagingKey = stagingKey;
        Status = UploadStatus.Pending;
    }

    /// <summary>
    ///     Registers a received photo for the check.
    /// </summary>
    public static MediaUpload Create(
        Guid uploadId,
        Guid requestId,
        Guid clientId,
        string fileName,
        string contentType,
        long sizeInBytes,
        string stagingKey,
        DateTimeOffset createdAt)
    {
        EnsureNotEmpty(uploadId, nameof(uploadId));
        EnsureNotEmpty(requestId, nameof(requestId));
        EnsureNotEmpty(clientId, nameof(clientId));

        var normalizedFileName = Path.GetFileName(fileName?.Trim() ?? string.Empty);
        var normalizedContentType = contentType?.Trim() ?? string.Empty;
        var normalizedKey = stagingKey?.Trim() ?? string.Empty;

        if (normalizedFileName.Length == 0 || normalizedFileName.Length > RequestRules.MaxFileNameLength)
        {
            throw new InvalidMediaUploadException(
                $"File name must contain from 1 to {RequestRules.MaxFileNameLength} characters.");
        }

        if (normalizedKey.Length == 0 || normalizedKey.Length > RequestRules.MaxStorageKeyLength)
        {
            throw new InvalidMediaUploadException(
                $"Storage key must contain from 1 to {RequestRules.MaxStorageKeyLength} characters.");
        }

        if (!RequestRules.AllowedMediaContentTypes.Contains(normalizedContentType))
        {
            throw new InvalidMediaUploadException(
                $"Content type '{normalizedContentType}' is not allowed, only photos are accepted.");
        }

        if (sizeInBytes <= 0 || sizeInBytes > RequestRules.MaxMediaFileSizeInBytes)
        {
            throw new InvalidMediaUploadException(
                $"Media file size must be from 1 to {RequestRules.MaxMediaFileSizeInBytes} bytes.");
        }

        var upload = new MediaUpload(
            uploadId,
            requestId,
            clientId,
            normalizedFileName,
            normalizedContentType,
            sizeInBytes,
            normalizedKey);

        upload.MarkAsCreated(createdAt);
        return upload;
    }

    /// <summary>
    ///     Marks the photo as accepted and attached to the request as the given media file.
    /// </summary>
    public void Complete(
        Guid mediaFileId,
        DateTimeOffset completedAt)
    {
        EnsureNotEmpty(mediaFileId, nameof(mediaFileId));
        EnsurePending("complete");

        Status = UploadStatus.Completed;
        MediaFileId = mediaFileId;
        ProcessedAt = completedAt;
        MarkAsUpdated(completedAt);
    }

    /// <summary>
    ///     Marks the photo as rejected.
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

    private static void EnsureNotEmpty(Guid identifier, string name)
    {
        if (identifier == Guid.Empty)
        {
            throw new InvalidIdentifierException(name);
        }
    }
}