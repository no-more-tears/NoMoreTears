using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveAvatar;

public sealed record RemoveAvatarCommand(
    Guid MasterId) : ICommand<MasterResponse>, IMasterOwnedRequest;
