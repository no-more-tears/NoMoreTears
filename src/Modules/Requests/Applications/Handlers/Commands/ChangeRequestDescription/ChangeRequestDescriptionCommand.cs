using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestDescription;

/// <summary>
///     Replaces the description of a draft.
/// </summary>
public sealed record ChangeRequestDescriptionCommand(
    Guid ClientId,
    Guid RequestId,
    string Description) : ICommand<RequestResponse>, IRequestClientRequest;