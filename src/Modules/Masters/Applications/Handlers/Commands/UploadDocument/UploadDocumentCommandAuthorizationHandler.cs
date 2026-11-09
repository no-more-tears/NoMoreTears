using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadDocument;

public sealed class UploadDocumentCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : MasterOwnerAuthorizationHandler<UploadDocumentCommand>(currentUser);
