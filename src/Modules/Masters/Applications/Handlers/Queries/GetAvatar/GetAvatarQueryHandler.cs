namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetAvatar;

public sealed class GetAvatarQueryHandler(
    IFileUploadService fileUploadService) : IQueryHandler<GetAvatarQuery, FileContentResponse>
{
    public async Task<FileContentResponse> Handle(
        GetAvatarQuery request,
        CancellationToken cancellationToken)
    {
        return await fileUploadService.OpenAvatarAsync(request.MasterId, cancellationToken);
    }
}
