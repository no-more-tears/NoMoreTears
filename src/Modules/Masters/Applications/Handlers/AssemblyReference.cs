using System.Reflection;

namespace NoMoreTears.Modules.Masters.Applications.Handlers;

/// <summary>
///     Represents a reference to an assembly.
/// </summary>
public sealed class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}