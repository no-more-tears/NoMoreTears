namespace NoMoreTears.Modules.Requests.Domain.Enums;

/// <summary>
///     Processing status of an uploaded photo.
/// </summary>
public enum UploadStatus
{
    /// <summary>
    ///     The file is received and waits for the check.
    /// </summary>
    Pending,

    /// <summary>
    ///     The file passed the check and is attached to the request.
    /// </summary>
    Completed,

    /// <summary>
    ///     The file was rejected or could not be attached.
    /// </summary>
    Failed
}