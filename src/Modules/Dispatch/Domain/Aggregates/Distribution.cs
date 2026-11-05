using NoMoreTears.Modules.Dispatch.Domain.Entities;
using NoMoreTears.Modules.Dispatch.Domain.Enums;
using NoMoreTears.Modules.Dispatch.Domain.Exceptions;
using NoMoreTears.Modules.Dispatch.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Aggregates;

/// <summary>
///     Distribution of one publication of a request.
/// </summary>
public sealed class Distribution : AuditableEntity<Guid>
{
    /// <summary>
    ///     Identifier of the request in Requests.
    /// </summary>
    public Guid RequestId { get; }

    /// <summary>
    ///     Publication number of the request; greater than 1 for a republication.
    /// </summary>
    public int PublicationNumber { get; }

    /// <summary>
    ///     Category of the request.
    /// </summary>
    public Guid CategoryId { get; }

    /// <summary>
    ///     Version of the dispatch policy the distribution was planned with.
    /// </summary>
    public Guid PolicyId { get; }

    /// <summary>
    ///     Moment after which the request cannot be accepted.
    /// </summary>
    public DateTimeOffset AcceptanceDeadline { get; }

    /// <summary>
    ///     Status of the distribution.
    /// </summary>
    public DistributionStatus Status { get; private set; }

    /// <summary>
    ///     Number of the latest opened wave.
    /// </summary>
    public int CurrentWave { get; private set; }

    /// <summary>
    ///     Moment the current wave opened.
    /// </summary>
    public DateTimeOffset WaveOpenedAt { get; private set; }

    /// <summary>
    ///     Master who accepted the request.
    /// </summary>
    public Guid? AcceptedMasterId { get; private set; }

    /// <summary>
    ///     Moment of acceptance.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; private set; }

    /// <summary>
    ///     Arrival interval chosen by the master.
    /// </summary>
    public TimeSpan? ArrivalInterval { get; private set; }

    /// <summary>
    ///     Moment the distribution was withdrawn.
    /// </summary>
    public DateTimeOffset? WithdrawnAt { get; private set; }

    private readonly List<Candidate> _candidates = [];

    /// <summary>
    ///     Planned candidates.
    /// </summary>
    public IReadOnlyCollection<Candidate> Candidates => _candidates.AsReadOnly();

    /// <summary>
    ///     Whether the request was published again after a failure.
    /// </summary>
    public bool IsRepublication => PublicationNumber > 1;

    private Distribution(
        Guid id,
        Guid requestId,
        int publicationNumber,
        Guid categoryId,
        Guid policyId,
        DateTimeOffset acceptanceDeadline,
        DateTimeOffset openedAt)
    {
        Id = id;
        RequestId = requestId;
        PublicationNumber = publicationNumber;
        CategoryId = categoryId;
        PolicyId = policyId;
        AcceptanceDeadline = acceptanceDeadline;
        Status = DistributionStatus.Open;
        CurrentWave = 1;
        WaveOpenedAt = openedAt;
    }

    /// <summary>
    ///     Plans a distribution and opens wave 1.
    /// </summary>
    public static Distribution Create(
        Guid requestId,
        int publicationNumber,
        Guid categoryId,
        DateTimeOffset acceptanceDeadline,
        DispatchPolicy policy,
        IReadOnlyCollection<CandidateProfile> profiles,
        DateTimeOffset openedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);
        ArgumentNullException.ThrowIfNull(profiles);
        EnsureNotEmpty(requestId, nameof(requestId));
        EnsureNotEmpty(categoryId, nameof(categoryId));
        ArgumentOutOfRangeException.ThrowIfLessThan(publicationNumber, 1);

        if (acceptanceDeadline <= openedAt)
        {
            throw new AcceptanceDeadlinePassedException();
        }

        var distribution = new Distribution(
            Guid.CreateVersion7(openedAt),
            requestId,
            publicationNumber,
            categoryId,
            policy.Id,
            acceptanceDeadline,
            openedAt);

        distribution.MarkAsCreated(openedAt);
        distribution.Plan(policy, profiles);
        distribution.OfferCurrentWave(openedAt);

        return distribution;
    }

    /// <summary>
    ///     Opens the next wave when the window of the current one has passed.
    /// </summary>
    public void AdvanceWave(
        DispatchPolicy policy,
        DateTimeOffset advancedAt)
    {
        ArgumentNullException.ThrowIfNull(policy);
        EnsureOpen("advance the wave of");
        EnsurePolicy(policy);

        var current = policy.WaveSettingsFor(CurrentWave);

        if (current.IsFinal)
        {
            throw new WaveAdvanceException("The current wave is final.");
        }

        if (advancedAt < WaveOpenedAt + current.Window)
        {
            throw new WaveAdvanceException($"The next wave opens at {WaveOpenedAt + current.Window:O}.");
        }

        CurrentWave++;
        WaveOpenedAt = advancedAt;
        OfferCurrentWave(advancedAt);
        MarkAsUpdated(advancedAt);
    }

    /// <summary>
    ///     Returns how long the master took to react since the request appeared in their feed.
    /// </summary>
    public TimeSpan ReactionTimeOf(
        Guid masterId,
        DateTimeOffset attemptedAt)
    {
        var offeredAt = OfferedCandidate(masterId).OfferedAt!.Value;

        return attemptedAt > offeredAt ? attemptedAt - offeredAt : TimeSpan.Zero;
    }

    /// <summary>
    ///     Returns the score of the request in the master's feed with its explanation.
    /// </summary>
    public Score ScoreFor(Guid masterId) => OfferedCandidate(masterId).Score;

    /// <summary>
    ///     Returns whether the request is in the master's feed.
    /// </summary>
    public bool IsOfferedTo(Guid masterId)
    {
        return Status is DistributionStatus.Open
            && _candidates.Exists(candidate => candidate.MasterId == masterId && candidate.IsOffered);
    }

    /// <summary>
    ///     Accepts the request exclusively for the master.
    /// </summary>
    public void Accept(
        Master master,
        TimeSpan arrivalInterval,
        DispatchPolicy policy,
        DateTimeOffset acceptedAt)
    {
        ArgumentNullException.ThrowIfNull(master);
        ArgumentNullException.ThrowIfNull(policy);

        if (Status is DistributionStatus.Accepted)
        {
            throw new RequestAlreadyAcceptedException();
        }

        EnsureOpen("accept");
        EnsurePolicy(policy);

        if (acceptedAt >= AcceptanceDeadline)
        {
            throw new AcceptanceDeadlinePassedException();
        }

        _ = OfferedCandidate(master.Id);

        if (!DispatchRules.AllowedArrivalIntervals.Contains(arrivalInterval))
        {
            throw new InvalidArrivalIntervalException(arrivalInterval);
        }

        master.EnsureCanTakeOn(policy, acceptedAt);
        master.TakeOn(RequestId, acceptedAt);

        Status = DistributionStatus.Accepted;
        AcceptedMasterId = master.Id;
        AcceptedAt = acceptedAt;
        ArrivalInterval = arrivalInterval;
        MarkAsUpdated(acceptedAt);
    }

    /// <summary>
    ///     Withdraws the request from all feeds after it was cancelled or expired in Requests.
    /// </summary>
    public void Withdraw(DateTimeOffset withdrawnAt)
    {
        EnsureOpen("withdraw");

        Status = DistributionStatus.Withdrawn;
        WithdrawnAt = withdrawnAt;
        MarkAsUpdated(withdrawnAt);
    }

    private void Plan(
        DispatchPolicy policy,
        IReadOnlyCollection<CandidateProfile> profiles)
    {
        var duplicate = profiles
            .GroupBy(profile => profile.MasterId)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new DuplicateCandidateException(duplicate.Key);
        }

        var eligible = profiles.Where(profile => profile.CanTakeOn).ToList();
        var cohortMedian = Median(eligible.Select(profile => (double)profile.Performance.AcceptedLastWeek).ToList());

        var remaining = eligible
            .Select(profile => new PlannedCandidate(
                profile,
                Score.Calculate(
                    ScoreFactors.Calculate(profile, cohortMedian),
                    policy.Weights,
                    profile.HasScorePenalty ? policy.RestrictionPenalty : 0),
                policy.NewcomerQuota.IsNewcomer(profile.Performance.CompletedRequests)))
            .OrderByDescending(planned => planned.Score.Total)
            .ThenBy(planned => planned.Profile.MasterId)
            .ToList();

        var total = remaining.Count;

        foreach (var wave in policy.Waves)
        {
            var slots = wave.CumulativeSizeFor(total) - _candidates.Count;

            if (slots <= 0)
            {
                continue;
            }

            var pool = remaining
                .Where(planned => wave.Number > 1 || !planned.Profile.IsExcludedFromWaveOne)
                .ToList();

            foreach (var picked in PickWithQuota(pool, slots, policy.NewcomerQuota))
            {
                remaining.Remove(picked);
                _candidates.Add(Candidate.Create(picked.Profile.MasterId, wave.Number, picked.IsNewcomer, picked.Score));
            }
        }

        if (IsRepublication)
        {
            foreach (var candidate in _candidates.Where(candidate => candidate.Wave == 1))
            {
                candidate.AddBonus(policy.RepublicationBonus);
            }
        }
    }

    private static List<PlannedCandidate> PickWithQuota(
        List<PlannedCandidate> pool,
        int slots,
        NewcomerQuota quota)
    {
        var picked = pool.Take(slots).ToList();
        var deficit = quota.ReservedSlotsOf(picked.Count) - picked.Count(planned => planned.IsNewcomer);

        if (deficit <= 0)
        {
            return picked;
        }

        var newcomers = pool.Skip(slots).Where(planned => planned.IsNewcomer).Take(deficit).ToList();
        var replaced = picked.Where(planned => !planned.IsNewcomer).Reverse().Take(newcomers.Count).ToList();

        return [.. picked.Except(replaced), .. newcomers];
    }

    private static double Median(List<double> values)
    {
        if (values.Count == 0)
        {
            return 0;
        }

        values.Sort();
        var middle = values.Count / 2;

        return values.Count % 2 == 1 ? values[middle] : (values[middle - 1] + values[middle]) / 2;
    }

    private void OfferCurrentWave(DateTimeOffset offeredAt)
    {
        foreach (var candidate in _candidates.Where(candidate => candidate.Wave == CurrentWave))
        {
            candidate.Offer(offeredAt);
        }
    }

    private Candidate OfferedCandidate(Guid masterId)
    {
        return _candidates.Find(candidate => candidate.MasterId == masterId && candidate.IsOffered)
            ?? throw new RequestNotOfferedException(masterId);
    }

    private void EnsureOpen(string action)
    {
        if (Status is not DistributionStatus.Open)
        {
            throw new InvalidDistributionStatusException(Status, action);
        }
    }

    private void EnsurePolicy(DispatchPolicy policy)
    {
        if (policy.Id != PolicyId)
        {
            throw new PolicyMismatchException(PolicyId, policy.Id);
        }
    }

    private static void EnsureNotEmpty(Guid identifier, string name)
    {
        if (identifier == Guid.Empty)
        {
            throw new InvalidIdentifierException(name);
        }
    }

    private sealed record PlannedCandidate(
        CandidateProfile Profile,
        Score Score, bool IsNewcomer);
}