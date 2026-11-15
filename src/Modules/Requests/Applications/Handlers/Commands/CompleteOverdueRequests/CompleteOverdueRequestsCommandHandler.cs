namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CompleteOverdueRequests;

public sealed class CompleteOverdueRequestsCommandHandler(
    IDeadlineService deadlineService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<CompleteOverdueRequestsCommand, int>
{
    public async Task<int> Handle(
        CompleteOverdueRequestsCommand request,
        CancellationToken cancellationToken)
    {
        var completed = await deadlineService.CompleteOverdueAsync(request.BatchSize, cancellationToken);

        foreach (var response in completed)
        {
            await eventPublisher.PublishTransitionAsync(RequestsEvents.Completed(response), response, cancellationToken);
        }

        return completed.Count;
    }
}