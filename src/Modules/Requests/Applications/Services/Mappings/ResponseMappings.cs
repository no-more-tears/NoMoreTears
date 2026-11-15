using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Entities;

namespace NoMoreTears.Modules.Requests.Applications.Services.Mappings;

/// <summary>
///     Manual mapping of domain models to responses.
/// </summary>
public static class ResponseMappings
{
    /// <summary>
    ///     Maps the full request, including the address.
    /// </summary>
    public static RequestResponse ToResponse(this Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return Map(
            request,
            new AddressResponse(
                request.Address.Value,
                new CoordinatesResponse(request.Address.Coordinates.Latitude, request.Address.Coordinates.Longitude)));
    }

    /// <summary>
    ///     Maps the request without the address, for masters to whom the contacts are not revealed.
    /// </summary>
    public static RequestResponse ToResponseWithoutContacts(this Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return Map(request, null);
    }

    /// <summary>
    ///     Status history in chronological order.
    /// </summary>
    public static IReadOnlyList<StatusChangeResponse> ToHistoryResponse(this Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return [.. Chronological(request).Select(change => change.ToResponse())];
    }

    public static StatusChangeResponse ToResponse(this StatusChange change)
    {
        ArgumentNullException.ThrowIfNull(change);

        return new StatusChangeResponse(
            change.Id,
            change.From?.ToString(),
            change.To.ToString(),
            change.Transition.ToString(),
            change.Initiator.Role.ToString(),
            change.Initiator.UserId,
            change.Reason?.Value,
            change.OccurredAt);
    }

    public static MediaFileResponse ToResponse(this MediaFile mediaFile)
    {
        ArgumentNullException.ThrowIfNull(mediaFile);

        return new MediaFileResponse(mediaFile.Id, mediaFile.ContentType, mediaFile.SizeInBytes, mediaFile.AttachedAt);
    }

    public static MediaUploadResponse ToResponse(this MediaUpload upload)
    {
        ArgumentNullException.ThrowIfNull(upload);

        return new MediaUploadResponse(
            upload.Id,
            upload.RequestId,
            upload.FileName,
            upload.ContentType,
            upload.SizeInBytes,
            upload.Status.ToString(),
            upload.FailureReason?.Value,
            upload.MediaFileId,
            upload.CreatedAt,
            upload.ProcessedAt);
    }

    private static RequestResponse Map(
        Request request,
        AddressResponse? address)
    {
        return new RequestResponse(
            request.Id,
            request.ClientId,
            request.CategoryId,
            address,
            request.Description.Value,
            request.Status.ToString(),
            request.PublicationCount,
            request.IsRepublished,
            request.PublishedAt,
            request.AcceptanceDeadline,
            request.Assignment is null
                ? null
                : new AssignmentResponse(
                    request.Assignment.MasterId,
                    request.Assignment.AcceptedAt,
                    request.Assignment.ExpectedArrivalAt,
                    request.Assignment.NoShowReportableFrom),
            request.ConfirmationDeadline,
            request.CompletedAt,
            request.Failure is null
                ? null
                : new FailureResponse(request.Failure.Kind.ToString(), request.Failure.MasterId, request.Failure.Reason?.Value),
            request.CancelledAt,
            request.CancellationReason?.Value,
            request.IncidentId,
            request.StatusBeforeDispute?.ToString(),
            [.. request.MediaFiles.OrderBy(mediaFile => mediaFile.AttachedAt).Select(mediaFile => mediaFile.ToResponse())],
            Chronological(request).Last().ToResponse(),
            request.CreatedAt,
            request.LastUpdatedAt);
    }

    private static IOrderedEnumerable<StatusChange> Chronological(Request request) =>
        request.StatusHistory
            .OrderBy(change => change.OccurredAt)
            .ThenBy(change => change.Id);
}