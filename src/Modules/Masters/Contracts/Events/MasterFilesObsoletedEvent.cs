using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     Internal event of the Masters module: stored files are no longer referenced (replaced avatar, removed document,
///     processed staging copy). Files are removed by the Masters consumers only after the change is committed.
/// </summary>
public sealed record MasterFilesObsoletedEvent(
    Guid MasterId,
    IReadOnlyList<StoredFileResponse> Files,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
