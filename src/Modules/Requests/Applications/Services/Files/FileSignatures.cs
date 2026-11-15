namespace NoMoreTears.Modules.Requests.Applications.Services.Files;

/// <summary>
///     Checks that the content of a photo starts with the signature of its declared media type,
///     so that a renamed executable or script is not accepted as a photo.
/// </summary>
public static class FileSignatures
{
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] Riff = "RIFF"u8.ToArray();
    private static readonly byte[] Webp = "WEBP"u8.ToArray();
    private static readonly byte[] FileTypeBox = "ftyp"u8.ToArray();

    private static readonly byte[][] HeifBrands =
    [
        "heic"u8.ToArray(),
        "heix"u8.ToArray(),
        "hevc"u8.ToArray(),
        "hevx"u8.ToArray(),
        "heim"u8.ToArray(),
        "heis"u8.ToArray(),
        "mif1"u8.ToArray(),
        "msf1"u8.ToArray()
    ];

    /// <summary>
    ///     Returns whether <paramref name="content"/> matches <paramref name="contentType"/>.
    ///     Unknown media types never match.
    /// </summary>
    public static bool Matches(ReadOnlySpan<byte> content, string contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        return contentType.ToUpperInvariant() switch
        {
            "IMAGE/JPEG" => content.StartsWith(Jpeg),
            "IMAGE/PNG" => content.StartsWith(Png),
            "IMAGE/WEBP" => content.Length >= 12 && content.StartsWith(Riff) && content[8..12].SequenceEqual(Webp),
            "IMAGE/HEIC" => IsHeif(content),
            _ => false
        };
    }

    /// <summary>
    ///     File extension used for stored objects of the media type.
    /// </summary>
    public static string ExtensionFor(string contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        return contentType.ToUpperInvariant() switch
        {
            "IMAGE/JPEG" => ".jpg",
            "IMAGE/PNG" => ".png",
            "IMAGE/WEBP" => ".webp",
            "IMAGE/HEIC" => ".heic",
            _ => string.Empty
        };
    }

    // ISO base media file: a box size (4 bytes), the "ftyp" box type and the major brand.
    private static bool IsHeif(ReadOnlySpan<byte> content)
    {
        if (content.Length < 12 || !content[4..8].SequenceEqual(FileTypeBox))
        {
            return false;
        }

        var brand = content[8..12];

        foreach (var heifBrand in HeifBrands)
        {
            if (brand.SequenceEqual(heifBrand))
            {
                return true;
            }
        }

        return false;
    }
}