namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestAddress;

public sealed class ChangeRequestAddressCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ChangeRequestAddressCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ChangeRequestAddressCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ChangeAddressAsync(request.ClientId, request.RequestId, request.Address, cancellationToken);

        await eventPublisher.PublishAsync(RequestsEvents.DraftUpdated(response), cancellationToken);

        return response;
    }
}