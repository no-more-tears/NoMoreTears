namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ConfirmCompletion;

public sealed class ConfirmCompletionCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ConfirmCompletionCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ConfirmCompletionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ConfirmCompletionAsync(request.ClientId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(RequestsEvents.Completed(response), response, cancellationToken);

        return response;
    }
}