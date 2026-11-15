namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestDescription;

public sealed class ChangeRequestDescriptionCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ChangeRequestDescriptionCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ChangeRequestDescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ChangeDescriptionAsync(request.ClientId, request.RequestId, request.Description, cancellationToken);

        await eventPublisher.PublishAsync(RequestsEvents.DraftUpdated(response), cancellationToken);

        return response;
    }
}