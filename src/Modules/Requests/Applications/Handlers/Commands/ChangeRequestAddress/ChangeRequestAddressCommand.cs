using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Requests.Contracts.Requests;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestAddress;

/// <summary>
///     Replaces the geocoded address of a draft.
/// </summary>
public sealed record ChangeRequestAddressCommand(
    Guid ClientId,
    Guid RequestId,
    AddressRequest Address) : ICommand<RequestResponse>, IRequestClientRequest;