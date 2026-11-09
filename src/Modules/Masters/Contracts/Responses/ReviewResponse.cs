namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Decision of a reviewer; <see cref="Reason"/> is set for a rejection.
/// </summary>
public sealed record ReviewResponse(
    Guid ReviewerId,
    DateTimeOffset ReviewedAt,
    string? Reason);
