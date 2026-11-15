namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ArchiveCategorySnapshot;

/// <summary>
///     Archives a category of the catalog snapshot. Sent by the Requests consumers.
/// </summary>
public sealed record ArchiveCategorySnapshotCommand(
    Guid CategoryId,
    DateTimeOffset ArchivedAt) : ICommand;