namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Sanction or temporary measure applied by Trust.
///     <see cref="Kind"/> is <c>Warning</c>, <c>Restriction</c>, <c>Suspension</c> or <c>Deverification</c>.
/// </summary>
public sealed record SanctionResponse(
    Guid Id,
    string Kind,
    DateTimeOffset ImposedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? LiftedAt,
    bool IsActive);
