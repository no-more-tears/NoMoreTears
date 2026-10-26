using System.Reflection;

namespace NoMoreTears.Modules.Identity.Applications.Handlers;

/// <summary>
///     Represents a reference to an assembly.
/// </summary>
public sealed class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}