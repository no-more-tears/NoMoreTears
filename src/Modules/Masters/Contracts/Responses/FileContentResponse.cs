namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Content of a stored file. The caller disposes <see cref="Content"/>.
/// </summary>
public sealed record FileContentResponse(
    Stream Content,
    string ContentType,
    string FileName);
