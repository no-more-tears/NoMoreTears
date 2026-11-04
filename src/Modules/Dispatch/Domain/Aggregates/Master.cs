using NoMoreTears.Modules.Dispatch.Domain.Entities;
using NoMoreTears.Modules.Dispatch.Domain.Enums;
using NoMoreTears.Modules.Dispatch.Domain.Exceptions;
using NoMoreTears.Modules.Dispatch.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Aggregates;

/// <summary>
///     Master as seen by Dispatch.
/// </summary>
public sealed class Master : AuditableEntity<Guid>
{
    /// <summary>
    ///     Indicators used for scoring.
    /// </summary>
    public Performance Performance { get; private set; }

    /// <summary>
    ///     End of the cooldown after a refusal by the master.
    /// </summary>
    public DateTimeOffset? CooldownUntil { get; private set; }

    /// <summary>
    ///     Moment of the latest acceptance.
    /// </summary>
    public DateTimeOffset? LastAcceptedAt { get; private set; }

    /// <summary>
    ///     Whether the master must pass MFA before accepting requests.
    /// </summary>
    public bool IsStepUpRequired { get; private set; }

    /// <summary>
    ///     Moment automation was last suspected.
    /// </summary>
    public DateTimeOffset? AutomationSuspectedAt { get; private set; }

    private readonly List<ActiveAssignment> _activeAssignments = [];

    /// <summary>
    ///     Requests accepted and not finished yet.
    /// </summary>
    public IReadOnlyCollection<ActiveAssignment> ActiveAssignments => _activeAssignments.AsReadOnly();

    private readonly List<AcceptanceAttempt> _acceptanceAttempts = [];

    /// <summary>
    ///     Latest acceptance attempts kept for the rate limit and automation detection.
    /// </summary>
    public IReadOnlyCollection<AcceptanceAttempt> AcceptanceAttempts => _acceptanceAttempts.AsReadOnly();

    private readonly List<Measure> _measures = [];

    /// <summary>
    ///     Measures applied by Trust.
    /// </summary>
    public IReadOnlyCollection<Measure> Measures => _measures.AsReadOnly();

    private Master(
        Guid id,
        Performance performance)
    {
        Id = id;
        Performance = performance;
    }

    /// <summary>
    ///     Creates the Dispatch view of a master, usually when Masters grants feed access.
    /// </summary>
    public static Master Create(
        Guid masterId, 
        DateTimeOffset createdAt)
    {
        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        var master = new Master(masterId, Performance.Initial);
        master.MarkAsCreated(createdAt);

        return master;
    }

    /// <summary>
    ///     Replaces the indicators after a relevant event in Trust or Requests.
    /// </summary>
    public void UpdatePerformance(
        Performance performance,
        DateTimeOffset updatedAt)
    {
        ArgumentNullException.ThrowIfNull(performance);

        Performance = performance;
        MarkAsUpdated(updatedAt);
    }

    /// <summary>
    ///     Builds the candidate profile of the master for a request.
    /// </summary>
    public CandidateProfile ProfileFor(
        double distanceInKilometers,
        double serviceRadiusInKilometers,
        DispatchPolicy policy,
        DateTimeOffset moment)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return CandidateProfile.Of(
            Id,
            Performance,
            distanceInKilometers,
            serviceRadiusInKilometers,
            CanTakeOnAt(policy, moment),
            IsExcludedFromWaveOneAt(moment),
            HasActiveMeasure(MeasureKind.Restriction, moment));
    }

    /// <summary>
    ///     Registers an attempt to accept a request and checks the rate limit and automation.
    ///     Persist the attempt even if the acceptance itself fails.
    /// </summary>
    public bool RegisterAcceptanceAttempt(
        TimeSpan reactionTime,
        DispatchPolicy policy,
        DateTimeOffset attemptedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);

        var windowStart = attemptedAt - DispatchRules.AcceptanceAttemptWindow;
        var limit = policy.Limits.AcceptanceAttemptsPerMinute;

        if (_acceptanceAttempts.Count(attempt => attempt.AttemptedAt > windowStart) >= limit)
        {
            throw new AcceptanceRateLimitExceededException(limit);
        }

        _acceptanceAttempts.Add(AcceptanceAttempt.Of(attemptedAt, reactionTime));
        ForgetOldAttempts(limit);
        MarkAsUpdated(attemptedAt);

        if (IsStepUpRequired || !LooksAutomated())
        {
            return false;
        }

        IsStepUpRequired = true;
        AutomationSuspectedAt = attemptedAt;

        return true;
    }

    /// <summary>
    ///     Clears the step-up requirement after the master passed MFA.
    /// </summary>
    public void ConfirmStepUp(DateTimeOffset confirmedAt)
    {
        IsStepUpRequired = false;
        _acceptanceAttempts.Clear();
        MarkAsUpdated(confirmedAt);
    }

    /// <summary>
    ///     Frees the slot of a request that was completed, failed by a no-show or cancelled by the client.
    /// </summary>
    public void Release(
        Guid requestId,
        DateTimeOffset releasedAt)
    {
        _activeAssignments.Remove(FindAssignment(requestId));
        MarkAsUpdated(releasedAt);
    }

    /// <summary>
    ///     Frees the slot of a request refused by the master and starts the cooldown.
    /// </summary>
    public void ReleaseAfterRefusal(
        Guid requestId,
        DispatchPolicy policy,
        DateTimeOffset refusedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);

        _activeAssignments.Remove(FindAssignment(requestId));

        var cooldownUntil = refusedAt + policy.Limits.CooldownAfterRefusal;

        if (CooldownUntil is null || cooldownUntil > CooldownUntil)
        {
            CooldownUntil = cooldownUntil;
        }

        MarkAsUpdated(refusedAt);
    }

    /// <summary>
    ///     Applies a measure from Trust.
    /// </summary>
    public void ApplyMeasure(
        Guid measureId,
        MeasureKind kind,
        DateTimeOffset startsAt,
        DateTimeOffset? endsAt)
    {
        if (_measures.Exists(measure => measure.Id == measureId))
        {
            throw new MeasureAlreadyAppliedException(measureId);
        }

        _measures.Add(Measure.Create(measureId, kind, startsAt, endsAt));
        MarkAsUpdated(startsAt);
    }

    /// <summary>
    ///     Lifts a measure, for example after a human review or an appeal.
    /// </summary>
    public void LiftMeasure(
        Guid measureId,
        DateTimeOffset liftedAt)
    {
        var measure = _measures.Find(applied => applied.Id == measureId)
            ?? throw new MeasureNotFoundException(measureId);

        measure.Lift(liftedAt);
        MarkAsUpdated(liftedAt);
    }

    /// <summary>
    ///     Returns whether the master is below the active requests limit and not cooling down.
    /// </summary>
    public bool CanTakeOnAt(
        DispatchPolicy policy,
        DateTimeOffset moment)
    {
        ArgumentNullException.ThrowIfNull(policy);
        return _activeAssignments.Count < policy.Limits.ActiveRequestsLimit && !IsCoolingDownAt(moment);
    }

    /// <summary>
    ///     Returns whether a cooldown is in effect: after a refusal or as a Trust measure.
    /// </summary>
    public bool IsCoolingDownAt(DateTimeOffset moment) => CooldownEndAt(moment) is not null;

    /// <summary>
    ///     Returns whether the master must not be offered requests in wave 1.
    /// </summary>
    public bool IsExcludedFromWaveOneAt(DateTimeOffset moment)
    {
        return HasActiveMeasure(MeasureKind.Restriction, moment) || HasActiveMeasure(MeasureKind.WaveOneExclusion, moment);
    }

    /// <summary>
    ///     Checks that the master can accept one more request.
    /// </summary>
    internal void EnsureCanTakeOn(
        DispatchPolicy policy,
        DateTimeOffset moment)
    {
        if (IsStepUpRequired)
        {
            throw new StepUpVerificationRequiredException();
        }

        if (CooldownEndAt(moment) is { } cooldownEnd)
        {
            throw new MasterCoolingDownException(cooldownEnd);
        }

        if (_activeAssignments.Count >= policy.Limits.ActiveRequestsLimit)
        {
            throw new ActiveRequestsLimitReachedException(policy.Limits.ActiveRequestsLimit);
        }

        if (HasActiveMeasure(MeasureKind.AcceptanceThrottle, moment)
            && LastAcceptedAt + policy.Limits.ThrottledAcceptanceInterval is { } nextAllowedAt
            && moment < nextAllowedAt)
        {
            throw new AcceptanceThrottledException(nextAllowedAt);
        }
    }

    /// <summary>
    ///     Takes on an accepted request. Called by <see cref="Distribution.Accept"/> after all checks.
    /// </summary>
    internal void TakeOn(
        Guid requestId,
        DateTimeOffset acceptedAt)
    {
        _activeAssignments.Add(ActiveAssignment.Of(requestId, acceptedAt));
        LastAcceptedAt = acceptedAt;
        MarkAsUpdated(acceptedAt);
    }

    private DateTimeOffset? CooldownEndAt(DateTimeOffset moment)
    {
        var ends = _measures
            .Where(measure => measure.Kind is MeasureKind.Cooldown && measure.IsActiveAt(moment))
            .Select(measure => measure.EndsAt ?? DateTimeOffset.MaxValue)
            .ToList();

        if (CooldownUntil > moment)
        {
            ends.Add(CooldownUntil.Value);
        }

        return ends.Count == 0 ? null : ends.Max();
    }

    private bool HasActiveMeasure(
        MeasureKind kind,
        DateTimeOffset moment)
    {
        return _measures.Exists(measure => measure.Kind == kind && measure.IsActiveAt(moment));
    }

    private bool LooksAutomated()
    {
        if (_acceptanceAttempts.Count < DispatchRules.AutomationSampleSize)
        {
            return false;
        }

        var reactions = _acceptanceAttempts
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .Take(DispatchRules.AutomationSampleSize)
            .Select(attempt => attempt.ReactionTime)
            .Order()
            .ToArray();

        var middle = reactions.Length / 2;
        var median = reactions.Length % 2 == 1
            ? reactions[middle]
            : (reactions[middle - 1] + reactions[middle]) / 2;

        return median < DispatchRules.AutomationReactionThreshold;
    }

    private void ForgetOldAttempts(int limit)
    {
        var keep = Math.Max(limit, DispatchRules.AutomationSampleSize);

        _acceptanceAttempts.Sort((left, right) => left.AttemptedAt.CompareTo(right.AttemptedAt));

        if (_acceptanceAttempts.Count > keep)
        {
            _acceptanceAttempts.RemoveRange(0, _acceptanceAttempts.Count - keep);
        }
    }

    private ActiveAssignment FindAssignment(Guid requestId)
    {
        return _activeAssignments.Find(assignment => assignment.RequestId == requestId)
            ?? throw new ActiveAssignmentNotFoundException(requestId);
    }
}