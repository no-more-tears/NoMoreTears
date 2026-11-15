using NoMoreTears.Modules.Requests.Domain;

namespace NoMoreTears.Modules.Requests.HostSide.Endpoints;

/// <summary>
///     Request size limits of upload endpoints: the file limit of the domain plus room for the multipart envelope.
/// </summary>
internal static class UploadLimits
{
    private const long MultipartOverheadInBytes = 64 * 1024;

    public const long MediaRequestSizeInBytes = RequestRules.MaxMediaFileSizeInBytes + MultipartOverheadInBytes;
}