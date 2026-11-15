namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ReportNoShow;

public sealed class ReportNoShowCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ReportNoShowCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ReportNoShowCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ReportNoShowAsync(request.ClientId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(RequestsEvents.Failed(response), response, cancellationToken);

        return response;
    }
}