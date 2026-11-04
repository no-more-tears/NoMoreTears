using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a distribution is processed with a policy version other than the one it was planned with.
/// </summary>
public sealed class PolicyMismatchException(Guid expectedPolicyId, Guid actualPolicyId)
    : DomainException($"The distribution uses policy '{expectedPolicyId}', but policy '{actualPolicyId}' was given.");