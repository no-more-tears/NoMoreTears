namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveCategorySnapshot;

/// <summary>
///     Creates or renames a category of the catalog snapshot. Sent by the Requests consumers.
/// </summary>
public sealed record SaveCategorySnapshotCommand(
    Guid CategoryId,
    string Name,
    DateTimeOffset ChangedAt) : ICommand;