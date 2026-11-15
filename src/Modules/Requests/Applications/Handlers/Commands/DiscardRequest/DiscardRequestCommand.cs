using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DiscardRequest;

/// <summary>
///     Deletes a draft; its photos are removed from the storage after the commit.
/// </summary>
public sealed record DiscardRequestCommand(
    Guid ClientId,
    Guid RequestId) : ICommand, IRequestClientRequest;