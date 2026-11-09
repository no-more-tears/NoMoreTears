namespace NoMoreTears.Modules.Masters.Applications.Services.Files;

/// <summary>
///     Checks that the content of a file starts with the signature of its declared media type,
///     so that a renamed executable or script is not accepted as a document or photo.
/// </summary>
public static class FileSignatures
{
    private static readonly byte[] Pdf = "%PDF-"u8.ToArray();
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] Riff = "RIFF"u8.ToArray();
    private static readonly byte[] Webp = "WEBP"u8.ToArray();

    /// <summary>
    ///     Returns whether <paramref name="content"/> matches <paramref name="contentType"/>.
    ///     Unknown media types never match.
    /// </summary>
    public static bool Matches(ReadOnlySpan<byte> content, string contentType)
    {
        return contentType.ToUpperInvariant() switch
        {
            "APPLICATION/PDF" => content.StartsWith(Pdf),
            "IMAGE/JPEG" => content.StartsWith(Jpeg),
            "IMAGE/PNG" => content.StartsWith(Png),
            "IMAGE/WEBP" => content.Length >= 12 && content.StartsWith(Riff) && content[8..12].SequenceEqual(Webp),
            _ => false
        };
    }

    /// <summary>
    ///     File extension used for stored objects of the media type.
    /// </summary>
    public static string ExtensionFor(string contentType)
    {
        return contentType.ToUpperInvariant() switch
        {
            "APPLICATION/PDF" => ".pdf",
            "IMAGE/JPEG" => ".jpg",
            "IMAGE/PNG" => ".png",
            "IMAGE/WEBP" => ".webp",
            _ => string.Empty
        };
    }
}