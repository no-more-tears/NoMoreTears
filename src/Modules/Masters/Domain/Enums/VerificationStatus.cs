namespace NoMoreTears.Modules.Masters.Domain.Enums;

/// <summary>
///     Verification status of a master profile.
/// </summary>
public enum VerificationStatus
{
    NotSubmitted,
    Pending,
    Verified,
    Rejected,
    Revoked
}