namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

/// <summary>
///     Thrown when a web module fails to mount its services or pipeline.
/// </summary>
public sealed class WebModuleException(string message)
    : Exception(message);