using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Entities;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;

namespace NoMoreTears.Modules.Masters.Applications.Services.Mappings;

/// <summary>
///     Manual mapping of domain models to responses.
/// </summary>
public static class ResponseMappings
{
    /// <summary>
    ///     Maps the profile; time-dependent flags are evaluated at <paramref name="now"/>.
    /// </summary>
    public static MasterResponse ToResponse(this Master master, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(master);

        return new MasterResponse(
            master.Id,
            master.FullName.FirstName,
            master.FullName.LastName,
            master.Avatar is null ? null : new AvatarResponse(master.Avatar.ContentType, master.Avatar.SizeInBytes),
            master.Verification.ToString(),
            master.VerificationReview?.ToResponse(),
            master.SubmittedAt,
            master.VerifiedAt,
            master.RevokedAt,
            master.ReverificationAllowedFrom,
            master.VerificationHold is null
                ? null
                : new VerificationHoldResponse(master.VerificationHold.Reason.Value, master.VerificationHold.PlacedAt),
            master.ServiceArea.ToResponse(),
            master.ServiceAreaStatus.ToString(),
            master.ServiceAreaReview?.ToResponse(),
            master.WorkSchedule.ToResponse(),
            master.IsAvailable,
            master.HasFeedAccessAt(now),
            master.IsRestrictedAt(now),
            master.IsSuspendedAt(now),
            [.. master.Categories.Select(category => category.ToResponse())],
            [.. master.Documents.OrderBy(document => document.UploadedAt).Select(document => document.ToResponse())],
            [.. master.Sanctions.OrderBy(sanction => sanction.ImposedAt).Select(sanction => sanction.ToResponse(now))],
            master.CreatedAt,
            master.LastUpdatedAt);
    }

    public static MasterCategoryResponse ToResponse(this MasterCategory category) =>
        new(category.CategoryId, category.AddedAt, category.Status.ToString(), category.Review?.ToResponse());

    public static DocumentResponse ToResponse(this VerificationDocument document) =>
        new(document.Id, document.Kind.ToString(), document.CategoryId, document.ContentType, document.SizeInBytes, document.UploadedAt);

    public static SanctionResponse ToResponse(this Sanction sanction, DateTimeOffset now) =>
        new(sanction.Id, sanction.Kind.ToString(), sanction.ImposedAt, sanction.ExpiresAt, sanction.LiftedAt, sanction.IsActiveAt(now));

    public static ReviewResponse ToResponse(this ReviewDecision review) =>
        new(review.ReviewerId, review.ReviewedAt, review.Reason?.Value);

    public static ServiceAreaResponse ToResponse(this ServiceArea area) =>
        new(
            area.Kind.ToString(),
            area.Center is null ? null : new CoordinatesResponse(area.Center.Latitude, area.Center.Longitude),
            area.RadiusInKilometers,
            [.. area.Boundary.Select(vertex => new CoordinatesResponse(vertex.Latitude, vertex.Longitude))]);

    public static WorkScheduleResponse ToResponse(this WorkSchedule schedule) =>
        new(
            schedule.TimeZoneId,
            [.. schedule.Hours.Select(interval => new WorkingHoursResponse(interval.Day, interval.Start, interval.End))]);

    public static CategoryResponse ToResponse(this Category category) =>
        new(
            category.Id,
            category.Name.Value,
            category.Description?.Value,
            category.Status.ToString(),
            category.CreatedAt,
            category.LastUpdatedAt);

    public static FileUploadResponse ToResponse(this FileUpload upload) =>
        new(
            upload.Id,
            upload.Purpose.ToString(),
            upload.CategoryId,
            upload.FileName,
            upload.ContentType,
            upload.SizeInBytes,
            upload.Status.ToString(),
            upload.FailureReason?.Value,
            upload.DocumentId,
            upload.CreatedAt,
            upload.ProcessedAt);

    /// <summary>
    ///     Latest moment until which an active sanction of the given kind lasts; <c>null</c> when none is active
    ///     and <see cref="DateTimeOffset.MaxValue"/> when an active sanction has no end.
    /// </summary>
    public static DateTimeOffset? ActiveUntil(this Master master, SanctionKind kind, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(master);

        var active = master.Sanctions
            .Where(sanction => sanction.Kind == kind && sanction.IsActiveAt(now))
            .ToList();

        return active.Count == 0
            ? null
            : active.Max(sanction => sanction.ExpiresAt ?? DateTimeOffset.MaxValue);
    }
}