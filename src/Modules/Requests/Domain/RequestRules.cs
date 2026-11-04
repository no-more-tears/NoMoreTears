using System.Collections.Frozen;

namespace NoMoreTears.Modules.Requests.Domain;

/// <summary>
///     Business limits of the Requests bounded context.
/// </summary>
public static class RequestRules
{
    /// <summary>
    ///     Minimum length of a request description.
    /// </summary>
    public const int MinDescriptionLength = 20;

    /// <summary>
    ///     Maximum length of a request description.
    /// </summary>
    public const int MaxDescriptionLength = 2000;

    /// <summary>
    ///     Maximum length of a formatted geocoded address.
    /// </summary>
    public const int MaxAddressLength = 500;

    /// <summary>
    ///     Maximum length of a reason given by a participant.
    /// </summary>
    public const int MaxReasonLength = 1000;

    /// <summary>
    ///     Maximum number of media files attached to one request.
    /// </summary>
    public const int MaxMediaFiles = 10;

    /// <summary>
    ///     Maximum size of one media file: 25 MB.
    /// </summary>
    public const long MaxMediaFileSizeInBytes = 25L * 1024 * 1024;

    /// <summary>
    ///     Maximum length of the object storage key of a media file.
    /// </summary>
    public const int MaxStorageKeyLength = 1024;

    /// <summary>
    ///     Default number of simultaneously published requests per client.
    /// </summary>
    public const int DefaultPublishedRequestsLimit = 3;

    /// <summary>
    ///     Default period during which a published request waits for acceptance.
    /// </summary>
    public static readonly TimeSpan DefaultAcceptanceWaitingPeriod = TimeSpan.FromHours(24);

    /// <summary>
    ///     Period after which a request marked as done is completed automatically.
    /// </summary>
    public static readonly TimeSpan CompletionConfirmationPeriod = TimeSpan.FromHours(48);

    /// <summary>
    ///     Buffer after the expected arrival time before a client can report a no-show.
    /// </summary>
    public static readonly TimeSpan NoShowBuffer = TimeSpan.FromMinutes(60);

    /// <summary>
    ///     Content types accepted for request media files (photos only).
    /// </summary>
    public static FrozenSet<string> AllowedMediaContentTypes { get; } = new[]
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}