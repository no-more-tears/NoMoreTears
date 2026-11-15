using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Requests.Contracts.Requests;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CreateRequest;

/// <summary>
///     Creates a draft request of the current client (FR-REQ-01).
/// </summary>
public sealed record CreateRequestCommand(
    Guid ClientId,
    Guid CategoryId,
    AddressRequest Address,
    string Description) : ICommand<RequestResponse>, IRequestClientRequest;