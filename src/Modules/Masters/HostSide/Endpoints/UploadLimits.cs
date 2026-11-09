using NoMoreTears.Modules.Masters.Domain;

namespace NoMoreTears.Modules.Masters.HostSide.Endpoints;

/// <summary>
///     Request size limits of upload endpoints: the file limit of the domain plus room for the multipart envelope.
/// </summary>
internal static class UploadLimits
{
    private const long MultipartOverheadInBytes = 64 * 1024;

    public const long AvatarRequestSizeInBytes = MasterRules.MaxAvatarSizeInBytes + MultipartOverheadInBytes;

    public const long DocumentRequestSizeInBytes = MasterRules.MaxDocumentSizeInBytes + MultipartOverheadInBytes;
}
