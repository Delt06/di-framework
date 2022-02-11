using System;
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

        IRegisteredContainerBuilder OnDidNotRegisterLast();

        /// <summary>
        ///     Registers a new dependency that will be created using the given delegate.
        /// </summary>
        /// <param name="factoryMethod">A delegate that creates a dependency.</param>
        /// <returns>The builder.</returns>
        IRegisteredContainerBuilder RegisterFromMethodAsDelegate([NotNull] Delegate factoryMethod);
    }
}