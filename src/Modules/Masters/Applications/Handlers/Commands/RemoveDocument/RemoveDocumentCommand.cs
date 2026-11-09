using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveDocument;

public sealed record RemoveDocumentCommand(
    Guid MasterId,
    Guid DocumentId) : ICommand<MasterResponse>, IMasterOwnedRequest;
