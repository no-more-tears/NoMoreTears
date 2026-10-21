namespace NoMoreTears.Shared.CQS.Extensions;

/// <summary>
///     Extension for working with handler registration.
/// </summary>
internal static class TypeExtensions
{
    /// <param name="type"><see cref="Type"/> to determine.</param>
    extension(Type type)
    {
        /// <summary>
        ///     Determines whether the specified type is an open generic type.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the type is a generic type definition or contains unassigned generic type parameters; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenGenericType()
            => type.IsGenericTypeDefinition || type.ContainsGenericParameters;

        /// <summary>
        ///     Determines whether the specified type is a concrete class (i.e., not abstract and not an interface).
        /// </summary>
        /// <returns><c>true</c> if the type is concrete; otherwise, <c>false</c>.</returns>
        public bool IsConcreteType()
            => type is { IsAbstract: false, IsInterface: false };
    }
}