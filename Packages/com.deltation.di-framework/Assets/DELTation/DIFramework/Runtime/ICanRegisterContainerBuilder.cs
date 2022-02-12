using System;
using System.Runtime.CompilerServices;
using DELTation.DIFramework.Dependencies;
using JetBrains.Annotations;

namespace DELTation.DIFramework
{
    public interface ICanRegisterContainerBuilder
    {
        /// <summary>
        ///     Registers a new dependency of the given type.
        ///     An instance of that type will be automatically created.
        /// </summary>
        /// <typeparam name="T">Type of the dependency.</typeparam>
        /// <returns>The builder.</returns>
        IRegisteredContainerBuilder Register<[MeansImplicitUse] T>() where T : class;

        /// <summary>
        ///     Registers a new dependency from the given object.
        /// </summary>
        /// <param name="obj">Object to register as dependency.</param>
        /// <returns>The builder.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="obj" /> is null.</exception>
        IRegisteredContainerBuilder Register([NotNull] object obj);

        /// <summary>
        ///     Registers a new dependency that will be created using the given delegate.
        /// </summary>
        /// <param name="factoryMethod">A delegate that creates a dependency.</param>
        /// <returns>The builder.</returns>
        IRegisteredContainerBuilder RegisterFromMethodAsDelegate([NotNull] Delegate factoryMethod);

        /// <summary>
        ///     Registers a new composite dependency.
        /// </summary>
        /// <param name="compositeDependency">The dependency to register.</param>
        /// <returns>The builder.</returns>
        IRegisteredContainerBuilder RegisterCompositeDependency([NotNull] CompositeDependency compositeDependency);
    }

    internal static class CanRegisterContainerBuilderInternalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegisteredContainerBuilder OnDidNotRegisterLast(
            [NotNull] this ICanRegisterContainerBuilder containerBuilder) =>
            containerBuilder switch
            {
                null => throw new ArgumentNullException(nameof(containerBuilder)),
                ContainerBuilder casterContainerBuilder => casterContainerBuilder.OnDidNotRegisterLast(),
                _ => throw new InvalidOperationException(
                    $"Provided container builder is of invalid type (${containerBuilder.GetType()})."
                ),
            };
    }
}