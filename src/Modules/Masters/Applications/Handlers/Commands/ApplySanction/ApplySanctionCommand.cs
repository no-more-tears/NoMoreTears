namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplySanction;

/// <summary>
///     Applies a sanction of Trust to a master. Sent by the Masters consumers.
/// </summary>
public sealed record ApplySanctionCommand(
    Guid MasterId,
    Guid SanctionId,
    string Kind,
    DateTimeOffset ImposedAt,
    DateTimeOffset? ExpiresAt) : ICommand;
