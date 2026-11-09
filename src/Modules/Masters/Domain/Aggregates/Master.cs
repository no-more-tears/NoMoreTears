using NoMoreTears.Modules.Masters.Domain.Entities;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Aggregates;

/// <summary>
///     Profile of a master.
/// </summary>
public sealed class Master: AuditableEntity<Guid>
{
    /// <summary>
    ///     First and last name of the master.
    /// </summary>
    public FullName FullName { get; private set; }

    /// <summary>
    ///     Profile photo.
    /// </summary>
    public Avatar? Avatar { get; private set; }

    /// <summary>
    ///     Verification status of the profile.
    /// </summary>
    public VerificationStatus Verification { get; private set; }

    /// <summary>
    ///     Latest decision on the profile as a whole.
    /// </summary>
    public ReviewDecision? VerificationReview { get; private set; }

    /// <summary>
    ///     Moment of the latest submission for verification.
    /// </summary>
    public DateTimeOffset? SubmittedAt { get; private set; }

    /// <summary>
    ///     Moment of the latest successful verification.
    /// </summary>
    public DateTimeOffset? VerifiedAt { get; private set; }

    /// <summary>
    ///     Moment the verification was revoked by a sanction.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    ///     Moment from which a master with revoked verification can apply again.
    /// </summary>
    public DateTimeOffset? ReverificationAllowedFrom { get; private set; }

    /// <summary>
    ///     Block of verification until a human check.
    /// </summary>
    public VerificationHold? VerificationHold { get; private set; }

    /// <summary>
    ///     Zone served by the master.
    /// </summary>
    public ServiceArea ServiceArea { get; private set; }

    /// <summary>
    ///     Review status of the service area.
    /// </summary>
    public ReviewStatus ServiceAreaStatus { get; private set; }

    /// <summary>
    ///     Latest decision on the service area.
    /// </summary>
    public ReviewDecision? ServiceAreaReview { get; private set; }

    /// <summary>
    ///     Weekly work schedule.
    /// </summary>
    public WorkSchedule WorkSchedule { get; private set; }

    /// <summary>
    ///     Availability switch of the master.
    /// </summary>
    public bool IsAvailable { get; private set; }

    private readonly List<MasterCategory> _categories = [];

    /// <summary>
    ///     Categories chosen by the master.
    /// </summary>
    public IReadOnlyCollection<MasterCategory> Categories => _categories.AsReadOnly();

    private readonly List<VerificationDocument> _documents = [];

    /// <summary>
    ///     Uploaded verification documents.
    /// </summary>
    public IReadOnlyCollection<VerificationDocument> Documents => _documents.AsReadOnly();

    private readonly List<Sanction> _sanctions = [];

    /// <summary>
    ///     Sanctions and temporary measures applied by Trust.
    /// </summary>
    public IReadOnlyCollection<Sanction> Sanctions => _sanctions.AsReadOnly();

    /// <summary>
    ///     Used only by persistence to materialize the profile.
    /// </summary>
    private Master()
    {
        FullName = null!;
        ServiceArea = null!;
        WorkSchedule = null!;
    }

    private Master(
        Guid id,
        FullName fullName,
        ServiceArea serviceArea,
        WorkSchedule workSchedule)
    {
        Id = id;
        FullName = fullName;
        ServiceArea = serviceArea;
        WorkSchedule = workSchedule;
        Verification = VerificationStatus.NotSubmitted;
        ServiceAreaStatus = ReviewStatus.Pending;
    }

    /// <summary>
    ///     Creates a master profile for a user with the Worker role.
    /// </summary>
    public static Master Create(
        Guid userId,
        FullName fullName,
        IReadOnlyCollection<Category> categories,
        ServiceArea serviceArea,
        WorkSchedule workSchedule,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(serviceArea);
        ArgumentNullException.ThrowIfNull(workSchedule);

        if (userId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(userId));
        }

        if (categories.Count == 0)
        {
            throw new MasterCategoryRequiredException();
        }

        var master = new Master(userId, fullName, serviceArea, workSchedule);
        master.MarkAsCreated(createdAt);

        foreach (var category in categories)
        {
            master.AttachCategory(category, createdAt);
        }

        return master;
    }

    /// <summary>
    ///     Changes the first and last name. The name is confirmed by the identity document, so it cannot change
    ///     while the profile is under review or verified; after a rejection or revocation it is checked on the next submission.
    /// </summary>
    public void ChangeFullName(
        FullName fullName,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        EnsureVerificationIn(
            "change the name of",
            VerificationStatus.NotSubmitted,
            VerificationStatus.Rejected,
            VerificationStatus.Revoked);

        FullName = fullName;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Sets or replaces the profile photo.
    /// </summary>
    public void ChangeAvatar(
        Avatar avatar,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(avatar);

        Avatar = avatar;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Removes the profile photo.
    /// </summary>
    public void RemoveAvatar(DateTimeOffset removedAt)
    {
        if (Avatar is null)
        {
            throw new InvalidAvatarException("The profile has no avatar.");
        }

        Avatar = null;
        MarkAsUpdated(removedAt);
    }

    /// <summary>
    ///     Adds a category.
    ///     After verification it awaits review while other verified categories keep working.
    /// </summary>
    public void AddCategory(
        Category category,
        DateTimeOffset addedAt)
    {
        ArgumentNullException.ThrowIfNull(category);

        AttachCategory(category, addedAt);
        MarkAsUpdated(addedAt);
    }

    /// <summary>
    ///     Removes a category. The profile always keeps at least one category.
    /// </summary>
    public void RemoveCategory(
        Guid categoryId,
        DateTimeOffset removedAt)
    {
        var category = FindCategory(categoryId);

        if (_categories.Count == 1)
        {
            throw new MasterCategoryRequiredException();
        }

        _categories.Remove(category);
        MarkAsUpdated(removedAt);
    }

    /// <summary>
    ///     Changes the service area. A changed area awaits review again.
    /// </summary>
    public void ChangeServiceArea(
        ServiceArea serviceArea,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(serviceArea);

        if (ServiceArea == serviceArea)
        {
            return;
        }

        ServiceArea = serviceArea;
        ServiceAreaStatus = ReviewStatus.Pending;
        ServiceAreaReview = null;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Changes the work schedule. The schedule does not require review.
    /// </summary>
    public void ChangeWorkSchedule(
        WorkSchedule workSchedule,
        DateTimeOffset changedAt)
    {
        ArgumentNullException.ThrowIfNull(workSchedule);

        WorkSchedule = workSchedule;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Turns availability on.
    /// </summary>
    public void BecomeAvailable(DateTimeOffset changedAt)
    {
        IsAvailable = true;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Turns availability off.
    /// </summary>
    public void BecomeUnavailable(DateTimeOffset changedAt)
    {
        IsAvailable = false;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Uploads an identity document.
    /// </summary>
    public VerificationDocument UploadIdentityDocument(
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAt)
    {
        return AttachDocument(
            VerificationDocument.Create(DocumentKind.Identity, null, storageKey, contentType, sizeInBytes, uploadedAt));
    }

    /// <summary>
    ///     Uploads a qualification document for a category of the profile.
    /// </summary>
    public VerificationDocument UploadQualificationDocument(
        Guid categoryId,
        string storageKey,
        string contentType,
        long sizeInBytes,
        DateTimeOffset uploadedAt)
    {
        _ = FindCategory(categoryId);

        return AttachDocument(
            VerificationDocument.Create(DocumentKind.Qualification, categoryId, storageKey, contentType, sizeInBytes, uploadedAt));
    }

    /// <summary>
    ///     Removes a document. Documents cannot be removed while the profile is under review.
    /// </summary>
    public void RemoveDocument(
        Guid documentId,
        DateTimeOffset removedAt)
    {
        if (Verification is VerificationStatus.Pending)
        {
            throw new InvalidVerificationStatusException(Verification, "remove a document of");
        }

        var document = _documents.Find(uploaded => uploaded.Id == documentId)
            ?? throw new DocumentNotFoundException(documentId);

        _documents.Remove(document);
        MarkAsUpdated(removedAt);
    }

    /// <summary>
    ///     Submits the profile for verification. After revocation it is allowed only after the waiting period
    ///     and only with documents uploaded after the revocation.
    /// </summary>
    public void SubmitForVerification(DateTimeOffset submittedAt)
    {
        EnsureVerificationIn(
            "submit for verification",
            VerificationStatus.NotSubmitted,
            VerificationStatus.Rejected,
            VerificationStatus.Revoked);

        var isReverification = Verification is VerificationStatus.Revoked;

        if (isReverification && submittedAt < ReverificationAllowedFrom)
        {
            throw new ReverificationNotAllowedYetException(ReverificationAllowedFrom!.Value);
        }

        var categoriesToReview = _categories
            .Where(category => isReverification || !category.IsVerified)
            .ToList();

        EnsureDocumentsUploaded(categoriesToReview);

        foreach (var category in categoriesToReview)
        {
            category.ReturnToReview();
        }

        if (isReverification || ServiceAreaStatus is not ReviewStatus.Verified)
        {
            ServiceAreaStatus = ReviewStatus.Pending;
            ServiceAreaReview = null;
        }

        Verification = VerificationStatus.Pending;
        VerificationReview = null;
        SubmittedAt = submittedAt;
        MarkAsUpdated(submittedAt);
    }

    /// <summary>
    ///     Confirms the qualification of the master in a category.
    /// </summary>
    public void VerifyCategory(
        Guid reviewerId,
        Guid categoryId, 
        DateTimeOffset verifiedAt)
    {
        EnsureVerificationIn(
            "verify a category of",
            VerificationStatus.Pending,
            VerificationStatus.Verified);
        EnsureNotOnHold();

        var category = FindCategory(categoryId);
        EnsureQualificationDocumentUploaded(categoryId);

        category.Verify(ReviewDecision.Of(reviewerId, verifiedAt, null));
        MarkAsUpdated(verifiedAt);
    }

    /// <summary>
    ///     Rejects the qualification of the master in a category with a reason.
    /// </summary>
    public void RejectCategory(
        Guid reviewerId,
        Guid categoryId,
        Reason reason,
        DateTimeOffset rejectedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsureVerificationIn("reject a category of", VerificationStatus.Pending, VerificationStatus.Verified);

        FindCategory(categoryId).Reject(ReviewDecision.Of(reviewerId, rejectedAt, reason));
        MarkAsUpdated(rejectedAt);
    }

    /// <summary>
    ///     Confirms the service area.
    /// </summary>
    public void VerifyServiceArea(
        Guid reviewerId,
        DateTimeOffset verifiedAt)
    {
        EnsureVerificationIn(
            "verify the service area of",
            VerificationStatus.Pending,
            VerificationStatus.Verified);
        EnsureNotOnHold();
        EnsureServiceAreaPending("verify");

        ServiceAreaStatus = ReviewStatus.Verified;
        ServiceAreaReview = ReviewDecision.Of(reviewerId, verifiedAt, null);
        MarkAsUpdated(verifiedAt);
    }

    /// <summary>
    ///     Rejects the service area with a reason.
    /// </summary>
    public void RejectServiceArea(
        Guid reviewerId,
        Reason reason,
        DateTimeOffset rejectedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);

        EnsureVerificationIn(
            "reject the service area of",
            VerificationStatus.Pending,
            VerificationStatus.Verified);
        EnsureServiceAreaPending("reject");

        ServiceAreaStatus = ReviewStatus.Rejected;
        ServiceAreaReview = ReviewDecision.Of(reviewerId, rejectedAt, reason);
        MarkAsUpdated(rejectedAt);
    }

    /// <summary>
    ///     Verifies the profile once every category and the service area are reviewed,
    ///     at least one category and the area are confirmed, and there is no hold.
    /// </summary>
    public void Verify(
        Guid reviewerId,
        DateTimeOffset verifiedAt)
    {
        EnsureVerificationIn("verify", VerificationStatus.Pending);
        EnsureNotOnHold();

        if (_categories.Exists(category => category.Status is ReviewStatus.Pending))
        {
            throw new VerificationIncompleteException("Every category must be reviewed before verification.");
        }

        if (!_categories.Exists(category => category.IsVerified))
        {
            throw new VerificationIncompleteException("At least one category must be verified.");
        }

        if (ServiceAreaStatus is not ReviewStatus.Verified)
        {
            throw new VerificationIncompleteException("The service area must be verified.");
        }

        Verification = VerificationStatus.Verified;
        VerificationReview = ReviewDecision.Of(reviewerId, verifiedAt, null);
        VerifiedAt = verifiedAt;
        ReverificationAllowedFrom = null;
        MarkAsUpdated(verifiedAt);
    }

    /// <summary>
    ///     Rejects the profile with a reason. The master can fix the profile and submit it again.
    /// </summary>
    public void Reject(
        Guid reviewerId,
        Reason reason,
        DateTimeOffset rejectedAt)
    {
        ArgumentNullException.ThrowIfNull(reason);
        EnsureVerificationIn("reject", VerificationStatus.Pending);

        Verification = VerificationStatus.Rejected;
        VerificationReview = ReviewDecision.Of(reviewerId, rejectedAt, reason);
        MarkAsUpdated(rejectedAt);
    }

    /// <summary>
    ///     Blocks verification until a human check, for example after a suspected duplicate account.
    /// </summary>
    public void PlaceVerificationHold(
        Reason reason,
        DateTimeOffset placedAt)
    {
        if (VerificationHold is not null)
        {
            throw new VerificationHoldException("Verification is already on hold.");
        }

        VerificationHold = VerificationHold.Of(reason, placedAt);
        MarkAsUpdated(placedAt);
    }

    /// <summary>
    ///     Releases the verification hold after a human check.
    /// </summary>
    public void ReleaseVerificationHold(DateTimeOffset releasedAt)
    {
        if (VerificationHold is null)
        {
            throw new VerificationHoldException("Verification is not on hold.");
        }

        VerificationHold = null;
        MarkAsUpdated(releasedAt);
    }

    /// <summary>
    ///     Records a warning: no change of access (sanction ladder, step 1).
    /// </summary>
    public void Warn(
        Guid sanctionId,
        DateTimeOffset imposedAt)
    {
        ApplySanction(Sanction.Create(sanctionId, SanctionKind.Warning, imposedAt, null));
    }

    /// <summary>
    ///     Restricts the master: excluded from wave 1 with lowered scoring, feed access is kept (step 2).
    /// </summary>
    public void Restrict(
        Guid sanctionId,
        DateTimeOffset imposedAt,
        DateTimeOffset expiresAt)
    {
        ApplySanction(Sanction.Create(sanctionId, SanctionKind.Restriction, imposedAt, expiresAt));
    }

    /// <summary>
    ///     Suspends feed access, as a sanction (step 3) or as a temporary measure until human review.
    ///     A measure without an end date lasts until it is lifted.
    /// </summary>
    public void Suspend(
        Guid sanctionId,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt)
    {
        ApplySanction(Sanction.Create(sanctionId, SanctionKind.Suspension, imposedAt, expiresAt));
    }

    /// <summary>
    ///     Revokes verification (step 4). 
    ///     Repeated verification is possible after the waiting period with new documents.
    /// </summary>
    public void RevokeVerification(
        Guid sanctionId,
        DateTimeOffset revokedAt)
    {
        EnsureVerificationIn("revoke verification of", VerificationStatus.Verified);

        ApplySanction(Sanction.Create(sanctionId, SanctionKind.Deverification, revokedAt, null));

        Verification = VerificationStatus.Revoked;
        RevokedAt = revokedAt;
        ReverificationAllowedFrom = revokedAt.AddMonths(MasterRules.ReverificationWaitingPeriodInMonths);
    }

    /// <summary>
    ///     Lifts a sanction, for example after an accepted appeal. Lifting a revocation restores the verification
    ///     if the master has not applied again yet.
    /// </summary>
    public void LiftSanction(
        Guid sanctionId,
        DateTimeOffset liftedAt)
    {
        var sanction = _sanctions.Find(applied => applied.Id == sanctionId)
            ?? throw new SanctionNotFoundException(sanctionId);

        sanction.Lift(liftedAt);

        if (sanction.Kind is SanctionKind.Deverification && Verification is VerificationStatus.Revoked)
        {
            Verification = VerificationStatus.Verified;
            RevokedAt = null;
            ReverificationAllowedFrom = null;
        }

        MarkAsUpdated(liftedAt);
    }

    /// <summary>
    ///     Returns whether the master has a confirmed qualification in the given category.
    /// </summary>
    public bool IsVerifiedFor(Guid categoryId)
    {
        return _categories.Exists(category => category.CategoryId == categoryId && category.IsVerified);
    }

    /// <summary>
    ///     Returns whether a sanction or measure with the given identifier has been applied.
    /// </summary>
    public bool HasSanction(Guid sanctionId) => _sanctions.Exists(applied => applied.Id == sanctionId);

    /// <summary>
    ///     Returns whether a restriction is in effect at the given moment.
    /// </summary>
    public bool IsRestrictedAt(DateTimeOffset moment) => HasActiveSanction(SanctionKind.Restriction, moment);

    /// <summary>
    ///     Returns whether a suspension is in effect at the given moment.
    /// </summary>
    public bool IsSuspendedAt(DateTimeOffset moment) => HasActiveSanction(SanctionKind.Suspension, moment);

    /// <summary>
    ///     Returns whether the master has access to the feed: verified, with at least one confirmed category
    ///     and a confirmed area, and not suspended.
    /// </summary>
    public bool HasFeedAccessAt(DateTimeOffset moment)
    {
        return !IsDeleted
            && Verification is VerificationStatus.Verified
            && _categories.Exists(category => category.IsVerified)
            && ServiceAreaStatus is ReviewStatus.Verified
            && !IsSuspendedAt(moment);
    }

    /// <summary>
    ///     Returns whether the master can be a Dispatch candidate at the given moment: feed access,
    ///     availability switched on and working hours.
    /// </summary>
    public bool CanReceiveRequestsAt(DateTimeOffset moment)
    {
        return IsAvailable && HasFeedAccessAt(moment) && WorkSchedule.IsWorkingAt(moment);
    }

    private IEnumerable<VerificationDocument> CurrentDocuments => RevokedAt is null
        ? _documents
        : _documents.Where(document => document.UploadedAt > RevokedAt);

    private void AttachCategory(
        Category category,
        DateTimeOffset addedAt)
    {
        if (!category.IsActive)
        {
            throw new CategoryArchivedException(category.Id);
        }

        if (_categories.Exists(chosen => chosen.CategoryId == category.Id))
        {
            throw new CategoryAlreadyAddedException(category.Id);
        }

        _categories.Add(MasterCategory.Create(category.Id, addedAt));
    }

    private VerificationDocument AttachDocument(VerificationDocument document)
    {
        if (_documents.Exists(uploaded => string.Equals(uploaded.StorageKey, document.StorageKey, StringComparison.Ordinal)))
        {
            throw new InvalidDocumentException($"Document '{document.StorageKey}' is already uploaded.");
        }

        _documents.Add(document);
        MarkAsUpdated(document.UploadedAt);

        return document;
    }

    private void ApplySanction(Sanction sanction)
    {
        if (_sanctions.Exists(applied => applied.Id == sanction.Id))
        {
            throw new SanctionAlreadyAppliedException(sanction.Id);
        }

        _sanctions.Add(sanction);
        MarkAsUpdated(sanction.ImposedAt);
    }

    private bool HasActiveSanction(
        SanctionKind kind,
        DateTimeOffset moment)
    {
        return _sanctions.Exists(sanction => sanction.Kind == kind && sanction.IsActiveAt(moment));
    }

    private MasterCategory FindCategory(Guid categoryId)
    {
        return _categories.Find(category => category.CategoryId == categoryId)
            ?? throw new MasterCategoryNotFoundException(categoryId);
    }

    private void EnsureDocumentsUploaded(IReadOnlyCollection<MasterCategory> categoriesToReview)
    {
        if (!CurrentDocuments.Any(document => document.Kind is DocumentKind.Identity))
        {
            throw new MissingVerificationDocumentsException("An identity document is required.");
        }

        foreach (var category in categoriesToReview)
        {
            EnsureQualificationDocumentUploaded(category.CategoryId);
        }
    }

    private void EnsureQualificationDocumentUploaded(Guid categoryId)
    {
        if (!CurrentDocuments.Any(document => document.ConfirmsQualificationIn(categoryId)))
        {
            throw new MissingVerificationDocumentsException(
                $"A qualification document is required for category '{categoryId}'.");
        }
    }

    private void EnsureServiceAreaPending(string action)
    {
        if (ServiceAreaStatus is not ReviewStatus.Pending)
        {
            throw new InvalidReviewStatusException(ServiceAreaStatus, action);
        }
    }

    private void EnsureNotOnHold()
    {
        if (VerificationHold is not null)
        {
            throw new VerificationHoldException("Verification is on hold until a human check.");
        }
    }

    private void EnsureVerificationIn(
        string action,
        params ReadOnlySpan<VerificationStatus> allowedStatuses)
    {
        foreach (var allowedStatus in allowedStatuses)
        {
            if (Verification == allowedStatus)
            {
                return;
            }
        }

        throw new InvalidVerificationStatusException(Verification, action);
    }
}