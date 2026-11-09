namespace NoMoreTears.Modules.Masters.Domain.Enums;

/// <summary>
///     Processing status of an uploaded file.
/// </summary>
public enum UploadStatus
{
    /// <summary>
    ///     The file is received and waits for the check.
    /// </summary>
    Pending,

    /// <summary>
    ///     The file passed the check and is attached to the profile.
    /// </summary>
    Completed,

    /// <summary>
    ///     The file was rejected or could not be attached.
    /// </summary>
    Failed
}
