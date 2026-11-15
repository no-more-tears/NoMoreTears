namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.PublishRequest;

public sealed class PublishRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<PublishRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        PublishRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.PublishAsync(request.ClientId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(RequestsEvents.Published(response), response, cancellationToken);

        return response;
    }
}