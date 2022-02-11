using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework
{
    /// <summary>
    ///     Utility methods for registering dependencies in ContainerBuilders.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        ///     Load and register a dependency from Resources folder.
        ///     <a href="https://docs.unity3d.com/ScriptReference/Resources.Load.html">See Resources.Load documentation.</a>
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="path">Path to the target resource to load.</param>
        /// <typeparam name="T">Type of loaded object.</typeparam>
        /// <returns>The same container builder.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     If <paramref name="containerBuilder" /> or <paramref name="path" /> is
        ///     null.
        /// </exception>
        public static ContainerBuilder RegisterFromResources<T>([NotNull] this ContainerBuilder containerBuilder,
            [NotNull] string path) where T : Object
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (path == null) throw new ArgumentNullException(nameof(path));

            var obj = Resources.Load<T>(path);
            return containerBuilder.Register(obj);
        }

        /// <summary>
        ///     Register the given dependency only if it is not null (it checks for Unity null too).
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="obj">Registered object.</param>
        /// <returns>The same container builder.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="containerBuilder" /> is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">If <paramref name="containerBuilder" /> is null.</exception>
        [NotNull]
        public static ContainerBuilder TryResolveGloballyAndRegister<[MeansImplicitUse] T>(
            [NotNull] this ContainerBuilder containerBuilder) where T : class
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            return Di.TryResolveGlobally(out T service) ? containerBuilder.Register(service) : containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR>([NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR> factoryMethod)
            where TR : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <typeparam name="T1">Dependency of the created object.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR, T1>([NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR, T1> factoryMethod)
            where TR : class
            where T1 : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <typeparam name="T1">Dependency of the created object.</typeparam>
        /// <typeparam name="T2">Dependency of the created object.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR, T1, T2>([NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR, T1, T2> factoryMethod)
            where TR : class
            where T1 : class
            where T2 : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <typeparam name="T1">Dependency of the created object.</typeparam>
        /// <typeparam name="T2">Dependency of the created object.</typeparam>
        /// <typeparam name="T3">Dependency of the created object.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR, T1, T2, T3>(
            [NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR, T1, T2, T3> factoryMethod) where TR : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <typeparam name="T1">Dependency of the created object.</typeparam>
        /// <typeparam name="T2">Dependency of the created object.</typeparam>
        /// <typeparam name="T3">Dependency of the created object.</typeparam>
        /// <typeparam name="T4">Dependency of the created object.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR, T1, T2, T3, T4>(
            [NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR, T1, T2, T3, T4> factoryMethod) where TR : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }

        /// <summary>
        ///     Register a dependency from the given factory method.
        /// </summary>
        /// <param name="containerBuilder">Container builder to register dependency in.</param>
        /// <param name="factoryMethod">Factory method that creates a dependency.</param>
        /// <typeparam name="TR">Dependency type.</typeparam>
        /// <typeparam name="T1">Dependency of the created object.</typeparam>
        /// <typeparam name="T2">Dependency of the created object.</typeparam>
        /// <typeparam name="T3">Dependency of the created object.</typeparam>
        /// <typeparam name="T4">Dependency of the created object.</typeparam>
        /// <typeparam name="T5">Dependency of the created object.</typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [NotNull]
        public static ContainerBuilder RegisterFromMethod<TR, T1, T2, T3, T4, T5>(
            [NotNull] this ContainerBuilder containerBuilder,
            [NotNull] FactoryMethod<TR, T1, T2, T3, T4, T5> factoryMethod) where TR : class
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            containerBuilder.RegisterFromMethod(factoryMethod);
            return containerBuilder;
        }
    }
}