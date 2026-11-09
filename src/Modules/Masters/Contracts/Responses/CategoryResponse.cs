namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Category of the catalog. <see cref="Status"/> is <c>Active</c> or <c>Archived</c>.
/// </summary>
public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? LastUpdatedAt);
