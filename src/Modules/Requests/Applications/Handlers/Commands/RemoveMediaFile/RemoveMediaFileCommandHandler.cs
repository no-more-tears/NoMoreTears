namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RemoveMediaFile;

public sealed class RemoveMediaFileCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<RemoveMediaFileCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        RemoveMediaFileCommand request,
        CancellationToken cancellationToken)
    {
        var removal = await requestService.DetachMediaFileAsync(request.ClientId, request.RequestId, request.MediaFileId, cancellationToken);
        var changedAt = RequestsEvents.ChangedAt(removal.Request);

        await eventPublisher.PublishAsync(RequestsEvents.DraftUpdated(removal.Request), cancellationToken);
        await eventPublisher.PublishObsoleteFilesAsync(removal.Request.Id, removal.RemovedFiles, changedAt, cancellationToken);

        return removal.Request;
    }
}