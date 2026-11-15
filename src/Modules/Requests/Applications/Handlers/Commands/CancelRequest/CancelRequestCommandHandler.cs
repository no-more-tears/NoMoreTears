using NoMoreTears.Modules.Requests.Domain.Enums;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CancelRequest;

public sealed class CancelRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<CancelRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        CancelRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.CancelAsync(request.ClientId, request.RequestId, request.Reason, cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestCancelledEvent(
                response.Id,
                response.ClientId,
                response.Assignment?.MasterId,
                response.LastStatusChange.Transition == nameof(StatusTransition.CancelledAfterAcceptance),
                response.CancellationReason,
                response.CancelledAt!.Value,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}