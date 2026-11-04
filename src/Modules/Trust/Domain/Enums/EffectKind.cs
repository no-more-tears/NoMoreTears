namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Effect of a temporary measure applied by other contexts.
/// </summary>
public enum EffectKind
{
    FeedSuspension,
    Cooldown,
    WaveOneExclusion,
    StepUpVerification,
    AcceptanceThrottle,
    ReviewFreeze,
    ReputationExclusion,
    VerificationHold,
    PublicationLimit,
    PhoneConfirmation
}