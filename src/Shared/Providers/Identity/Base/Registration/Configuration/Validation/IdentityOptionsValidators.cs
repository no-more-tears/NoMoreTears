using Microsoft.Extensions.Options;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration.Validation;

/// <summary>
///     Source-generated validator for <see cref="JwtOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class JwtOptionsValidator : IValidateOptions<JwtOptions>;

/// <summary>
///     Source-generated validator for <see cref="SessionOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class SessionOptionsValidator : IValidateOptions<SessionOptions>;

/// <summary>
///     Source-generated validator for <see cref="MfaOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class MfaOptionsValidator : IValidateOptions<MfaOptions>;

/// <summary>
///     Source-generated validator for <see cref="UserActionTokenOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class UserActionTokenOptionsValidator : IValidateOptions<UserActionTokenOptions>;

/// <summary>
///     Source-generated validator for <see cref="PasswordOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class PasswordOptionsValidator : IValidateOptions<PasswordOptions>;

/// <summary>
///     Source-generated validator for <see cref="AccountOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class AccountOptionsValidator : IValidateOptions<AccountOptions>;

/// <summary>
///     Source-generated validator for <see cref="MaintenanceOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class MaintenanceOptionsValidator : IValidateOptions<MaintenanceOptions>;