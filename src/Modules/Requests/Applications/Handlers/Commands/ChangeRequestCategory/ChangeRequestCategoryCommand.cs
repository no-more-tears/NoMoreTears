using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestCategory;

/// <summary>
///     Chooses another category of the catalog for a draft.
/// </summary>
public sealed record ChangeRequestCategoryCommand(
    Guid ClientId,
    Guid RequestId,
    Guid CategoryId) : ICommand<RequestResponse>, IRequestClientRequest;