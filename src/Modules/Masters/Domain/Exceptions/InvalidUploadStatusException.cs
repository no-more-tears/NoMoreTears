using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an upload is finished twice.
/// </summary>
public sealed class InvalidUploadStatusException(UploadStatus status, string action)
    : DomainException($"Cannot {action} an upload with status '{status}'.");
