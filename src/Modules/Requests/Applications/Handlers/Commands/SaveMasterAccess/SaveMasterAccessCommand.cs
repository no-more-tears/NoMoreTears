namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveMasterAccess;

/// <summary>
///     Replaces the access snapshot of a master. Sent by the Requests consumers.
/// </summary>
public sealed record SaveMasterAccessCommand(
    Guid MasterId,
    IReadOnlyList<Guid> AcceptableCategoryIds,
    DateTimeOffset? SuspendedUntil,
    DateTimeOffset ChangedAt) : ICommand;