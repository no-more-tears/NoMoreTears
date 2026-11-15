namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestCategory;

public sealed class ChangeRequestCategoryCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<ChangeRequestCategoryCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        ChangeRequestCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var response = await requestService.ChangeCategoryAsync(request.ClientId, request.RequestId, request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(RequestsEvents.DraftUpdated(response), cancellationToken);

        return response;
    }
}