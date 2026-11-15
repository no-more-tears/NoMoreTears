namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExpireOverdueRequests;

public sealed class ExpireOverdueRequestsCommandHandler(
    IDeadlineService deadlineService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ExpireOverdueRequestsCommand, int>
{
    public async Task<int> Handle(
        ExpireOverdueRequestsCommand request,
        CancellationToken cancellationToken)
    {
        var expired = await deadlineService.ExpireOverdueAsync(request.BatchSize, cancellationToken);

        foreach (var response in expired)
        {
            await eventPublisher.PublishTransitionAsync(RequestsEvents.Expired(response), response, cancellationToken);
        }

        return expired.Count;
    }
}