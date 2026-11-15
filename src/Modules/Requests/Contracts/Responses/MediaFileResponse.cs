namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Photo attached to a request.
/// </summary>
public sealed record MediaFileResponse(
    Guid Id,
    string ContentType,
    long SizeInBytes,
    DateTimeOffset AttachedAt);