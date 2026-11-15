namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ApplyClientRestriction;

/// <summary>
///     Applies a publication restriction of Trust to a client (FR-REQ-09). Sent by the Requests consumers.
/// </summary>
public sealed record ApplyClientRestrictionCommand(
    Guid RestrictionId,
    Guid ClientId,
    string Kind,
    DateTimeOffset ImposedAt,
    DateTimeOffset? ExpiresAt) : ICommand;