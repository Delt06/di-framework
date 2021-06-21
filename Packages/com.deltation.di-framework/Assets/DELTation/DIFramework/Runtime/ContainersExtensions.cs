using System;
using System.Collections.Generic;
using DELTation.DIFramework.Containers;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework
{
    public static class ContainersExtensions
    {
        /// <summary>
        /// Specifies whether the given object should be ignored by a container.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns>True is the <paramref name="obj"/> should be ignored, false otherwise.</returns>
        public static bool ShouldBeIgnoredByContainer(object obj) =>
            obj is IIgnoreByContainer || obj is IDependencyContainer ||
            obj is Component c && (c == null || c.TryGetComponent(out IIgnoreByContainer _));

        /// <summary>
        /// Tries to resolve a dependency via the given container.
        /// </summary>
        /// <param name="container">Container to get dependencies from.</param>
        /// <param name="component">Resolved dependency (if found).</param>
        /// <typeparam name="T">Type of the dependency.</typeparam>
        /// <returns>True if resolved, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="container"/> is null.</exception>
        public static bool TryResolve<T>([NotNull] this IDependencyContainer container, out T component) where T : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (container.TryResolve(typeof(T), out var resolvedComponent))
            {
                component = (T) resolvedComponent;
                return true;
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Returns a collection of all (unique) objects registered in the container.
        /// </summary>
        /// <returns>A collection of all registered objects.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is null.</exception>
        public static IEnumerable<object> GetAllRegisteredObjects([NotNull] this IDependencyContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            var objects = new HashSet<object>();
            container.GetAllRegisteredObjects(objects);
            return objects;
        }
    }
}