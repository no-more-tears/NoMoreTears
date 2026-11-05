using NoMoreTears.Modules.Trust.Domain.Enums;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Definition of an automatic blocking rule:
///     whom it targets, which effects it has and how long it lasts.
/// </summary>
public sealed record RuleDefinition
{
    /// <summary>
    ///     The rule.
    /// </summary>
    public AutomaticRule Rule { get; }

    /// <summary>
    ///     Role of the participant the rule targets.
    /// </summary>
    public PartyRole SubjectRole { get; }

    /// <summary>
    ///     Validity of the measure.
    /// </summary>
    public TimeSpan? Duration { get; }

    /// <summary>
    ///     Effects of the measure.
    /// </summary>
    public IReadOnlyList<MeasureEffect> Effects { get; }

    private RuleDefinition(
        AutomaticRule rule,
        PartyRole subjectRole,
        TimeSpan? duration,
        IReadOnlyList<MeasureEffect> effects)
    {
        Rule = rule;
        SubjectRole = subjectRole;
        Duration = duration;
        Effects = effects;
    }

    /// <summary>
    ///     Returns the definition of the given rule.
    /// </summary>
    public static RuleDefinition Of(AutomaticRule rule)
    {
        return rule switch
        {
            AutomaticRule.RepeatedNoShows => new(
                rule,
                PartyRole.Master,
                TimeSpan.FromDays(7),
                [MeasureEffect.Of(EffectKind.FeedSuspension, null)]),
            AutomaticRule.MasterCancellationSeries => new(
                rule,
                PartyRole.Master,
                TimeSpan.FromDays(7),
                [MeasureEffect.Of(EffectKind.Cooldown, TimeSpan.FromHours(24)), MeasureEffect.Of(EffectKind.WaveOneExclusion, null)]),
            AutomaticRule.SuspectedAutomation => new(
                rule,
                PartyRole.Master,
                null,
                [MeasureEffect.Of(EffectKind.StepUpVerification, null), MeasureEffect.Of(EffectKind.AcceptanceThrottle, null)]),
            AutomaticRule.SelfBoosting => new(
                rule,
                PartyRole.Master,
                null,
                [MeasureEffect.Of(EffectKind.ReviewFreeze, null), MeasureEffect.Of(EffectKind.ReputationExclusion, null)]),
            AutomaticRule.DuplicateAccount => new(
                rule,
                PartyRole.Master,
                null,
                [MeasureEffect.Of(EffectKind.VerificationHold, null)]),
            AutomaticRule.ClientAbuse => new(
                rule,
                PartyRole.Client,
                TimeSpan.FromDays(30),
                [MeasureEffect.Of(EffectKind.PublicationLimit, null), MeasureEffect.Of(EffectKind.PhoneConfirmation, null)]),
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, "Unknown automatic rule.")
        };
    }

    /// <inheritdoc />
    public bool Equals(RuleDefinition? other) => other is not null && Rule == other.Rule;

    /// <inheritdoc />
    public override int GetHashCode() => Rule.GetHashCode();
}