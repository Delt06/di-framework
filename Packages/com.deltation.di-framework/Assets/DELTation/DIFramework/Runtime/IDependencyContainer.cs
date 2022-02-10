using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DELTation.DIFramework
{
    /// <summary>
    ///     Represents a container with registered dependencies.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        ///     Tries to resolve a dependency.
        /// </summary>
        /// <param name="type">Type of the dependency</param>
        /// <param name="dependency">Resolved dependency.</param>
        /// <returns>True if resolved, false otherwise.</returns>
        bool TryResolve([NotNull] Type type, out object dependency);

        /// <summary>
        ///     Checks whether the given type can be resolved via the container.
        ///     Produces no side effects.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type can be resolved, false otherwise.</returns>
        bool CanBeResolvedSafe([NotNull] Type type);


        /// <summary>
        ///     Populates the provided set with a collection of all objects registered in the container.
        ///     <seealso cref="Di.GetAllRegisteredObjects" />
        /// </summary>
        /// <param name="objects">Collection to put registered objects into.</param>
        void GetAllRegisteredObjects([NotNull] ICollection<object> objects);
    }
}