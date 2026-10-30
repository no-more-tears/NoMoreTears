namespace NoMoreTears.Shared.CQS.Primitives.Attributes;

/// <summary>
///     Marks a property whose value must never be written to logs.
/// </summary>
/// <remarks>
///     Text properties whose names contain <c>Password</c>, <c>Token</c>, <c>Secret</c>, <c>ApiKey</c> or
///     <c>Credential</c> are masked without the attribute. On positional records apply it
///     with the <c>property:</c> target: <c>[property: SensitiveData] string Code</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class SensitiveDataAttribute : Attribute;
