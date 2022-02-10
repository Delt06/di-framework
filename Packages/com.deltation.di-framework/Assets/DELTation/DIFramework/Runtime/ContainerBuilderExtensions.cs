using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace DELTation.DIFramework
{
    /// <summary>
    ///     Utility methods for registering dependencies in ContainerBuilders.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        ///     Register the given dependency only if it is not null (it checks for Unity null too).
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="obj">Registered object.</param>
        /// <returns>The same container builder.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="containerBuilder" /> is null.</exception>
        [NotNull]
        public static ContainerBuilder RegisterIfNotNull([NotNull] this ContainerBuilder containerBuilder,
            [CanBeNull] object obj)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (UnityUtils.IsNullOrUnityNull(obj)) return containerBuilder;

            Assert.IsNotNull(obj);
            return containerBuilder.Register(obj);
        }

        /// <summary>
        ///     Try to resolve a dependency globally. If resolved, register it.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <typeparam name="T">Type of dependency to try to resolve.</typeparam>
        /// <returns>The same container builder.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="containerBuilder" /> is null.</exception>
        [NotNull]
        public static ContainerBuilder TryResolveGloballyAndRegister<[MeansImplicitUse] T>(
            [NotNull] this ContainerBuilder containerBuilder) where T : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            return Di.TryResolveGlobally(out T service) ? containerBuilder.Register(service) : containerBuilder;
        }
    }
}