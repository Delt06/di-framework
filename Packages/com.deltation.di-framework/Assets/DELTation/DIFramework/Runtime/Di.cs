using System;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework
{
    /// <summary>
    /// Facade for the generic dependency resolution functions
    /// </summary>
    public static class Di
    {
        /// <summary>
        /// Try to resolve a dependency globally.
        /// </summary>
        /// <param name="result">Resolution result (set only if succeeded).</param>
        /// <typeparam name="T">Dependency type.</typeparam>
        /// <returns>True if resolved, false otherwise.</returns>
        public static bool TryResolveGlobally<T>(out T result) where T : class
        {
            var type = typeof(T);
            if (TryResolveGlobally(type, out var resultAsObject))
            {
                result = (T) resultAsObject;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Try to resolve a dependency globally.
        /// </summary>
        /// <param name="type">Dependency type.</param>
        /// <param name="result">Resolution result (set only if succeeded).</param>
        /// <returns>True if resolved, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        public static bool TryResolveGlobally([NotNull] Type type, out object result)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return DependencySource.Global.TryResolveGlobally(type, out result, out _);
        }

        /// <summary>
        /// Estimate whether the dependency will be resolved globally in runtime.
        /// </summary>
        /// <typeparam name="T">Dependency type.</typeparam>
        /// <returns>True if can be resolved, false otherwise.</returns>
        public static bool CanBeResolvedGloballySafe<T>() => CanBeResolvedGloballySafe(typeof(T));

        /// <summary>
        /// Estimate whether the dependency will be resolved globally in runtime.
        /// </summary>
        /// <param name="type">Dependency type.</param>
        /// <returns>True if can be resolved, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        public static bool CanBeResolvedGloballySafe([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return DependencySource.Global.CanBeResolvedGloballySafe(type, out _);
        }
    }
}