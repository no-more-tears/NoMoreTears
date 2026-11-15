namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RepublishRequest;

public sealed class RepublishRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<RepublishRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        RepublishRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.RepublishAsync(request.ClientId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(RequestsEvents.Published(response), response, cancellationToken);

        return response;
    }
}