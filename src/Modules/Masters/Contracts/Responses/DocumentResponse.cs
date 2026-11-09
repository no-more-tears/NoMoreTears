namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Verification document. <see cref="Kind"/> is <c>Identity</c> or <c>Qualification</c>.
/// </summary>
public sealed record DocumentResponse(
    Guid Id,
    string Kind,
    Guid? CategoryId,
    string ContentType,
    long SizeInBytes,
    DateTimeOffset UploadedAt);
