using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Internal event of the Requests module: stored files are no longer referenced (removed photo, discarded draft, processed staging copy). The Requests consumers remove them after the change is committed.
/// </summary>
public sealed record RequestFilesObsoletedEvent(
    Guid RequestId,
    IReadOnlyList<StoredFileResponse> Files,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);