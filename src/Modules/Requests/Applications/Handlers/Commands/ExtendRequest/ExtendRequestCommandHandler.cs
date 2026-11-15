namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExtendRequest;

public sealed class ExtendRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ExtendRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ExtendRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ExtendAsync(request.ClientId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestPublicationExtendedEvent(
                response.Id,
                response.ClientId,
                response.CategoryId,
                response.AcceptanceDeadline!.Value,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}