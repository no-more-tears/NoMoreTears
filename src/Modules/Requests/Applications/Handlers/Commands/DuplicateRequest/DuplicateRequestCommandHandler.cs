namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DuplicateRequest;

public sealed class DuplicateRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<DuplicateRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        DuplicateRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.DuplicateAsync(request.ClientId, request.RequestId, request.Description, cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestCreatedEvent(response.Id, response.ClientId, response.CategoryId, request.RequestId, RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}