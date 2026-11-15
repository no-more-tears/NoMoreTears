namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ObjectToCompletion;

public sealed class ObjectToCompletionCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ObjectToCompletionCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ObjectToCompletionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ObjectToCompletionAsync(request.ClientId, request.RequestId, request.Reason, cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestCompletionObjectedEvent(
                response.Id,
                response.ClientId,
                response.Assignment!.MasterId,
                response.LastStatusChange.Reason ?? string.Empty,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}