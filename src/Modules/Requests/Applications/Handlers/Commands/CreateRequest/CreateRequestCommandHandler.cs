namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CreateRequest;

public sealed class CreateRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<CreateRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        CreateRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.CreateAsync(
            request.ClientId,
            request.CategoryId,
            request.Address,
            request.Description,
            cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestCreatedEvent(response.Id, response.ClientId, response.CategoryId, null, RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}