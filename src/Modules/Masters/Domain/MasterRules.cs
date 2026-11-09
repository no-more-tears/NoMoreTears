using System.Collections.Frozen;

namespace NoMoreTears.Modules.Masters.Domain;

/// <summary>
///     Business limits of the Masters bounded context.
/// </summary>
public static class MasterRules
{
    /// <summary>
    ///     Minimum length of a category name.
    /// </summary>
    public const int MinCategoryNameLength = 2;

    /// <summary>
    ///     Maximum length of a category name.
    /// </summary>
    public const int MaxCategoryNameLength = 100;

    /// <summary>
    ///     Maximum length of a category description.
    /// </summary>
    public const int MaxCategoryDescriptionLength = 1000;

    /// <summary>
    ///     Maximum length of a reason.
    /// </summary>
    public const int MaxReasonLength = 1000;

    /// <summary>
    ///     Maximum radius of a service area in kilometers.
    /// </summary>
    public const double MaxServiceRadiusInKilometers = 100;

    /// <summary>
    ///     Minimum number of vertices of a polygon service area.
    /// </summary>
    public const int MinPolygonVertices = 3;

    /// <summary>
    ///     Maximum number of vertices of a polygon service area.
    /// </summary>
    public const int MaxPolygonVertices = 100;

    /// <summary>
    ///     Maximum size of one verification document: 25 MB.
    /// </summary>
    public const long MaxDocumentSizeInBytes = 25L * 1024 * 1024;

    /// <summary>
    ///     Maximum length of a first or last name.
    /// </summary>
    public const int MaxNamePartLength = 100;

    /// <summary>
    ///     Maximum size of an avatar: 5 MB.
    /// </summary>
    public const long MaxAvatarSizeInBytes = 5L * 1024 * 1024;

    /// <summary>
    ///     Maximum length of the object storage key of a document or an avatar.
    /// </summary>
    public const int MaxStorageKeyLength = 1024;

    /// <summary>
    ///     Maximum length of the original name of an uploaded file.
    /// </summary>
    public const int MaxFileNameLength = 255;

    /// <summary>
    ///     Duration of a restriction (sanction ladder, step 2) when Trust does not give an end date.
    /// </summary>
    public const int DefaultRestrictionDurationInDays = 7;

    /// <summary>
    ///     Months after revocation of verification before a master can apply again.
    /// </summary>
    public const int ReverificationWaitingPeriodInMonths = 12;

    /// <summary>
    ///     Content types accepted for verification documents.
    /// </summary>
    public static FrozenSet<string> AllowedDocumentContentTypes { get; } = new[]
    {
        "application/pdf",
        "image/jpeg",
        "image/png"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Content types accepted for avatars (photos only).
    /// </summary>
    public static FrozenSet<string> AllowedAvatarContentTypes { get; } = new[]
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}