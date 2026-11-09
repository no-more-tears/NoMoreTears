using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Masters.Domain.Enums;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.SubmitForVerification;

public sealed record SubmitForVerificationCommand(
    Guid MasterId) : ICommand<MasterResponse>, IMasterOwnedRequest;
