namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ActivateCategorySnapshot;

/// <summary>
///     Activates a category of the catalog snapshot. Sent by the Requests consumers.
/// </summary>
public sealed record ActivateCategorySnapshotCommand(
    Guid CategoryId,
    DateTimeOffset ActivatedAt) : ICommand;