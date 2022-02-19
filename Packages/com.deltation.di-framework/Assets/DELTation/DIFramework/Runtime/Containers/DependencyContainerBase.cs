using System;
using System.Collections.Generic;
using DELTation.DIFramework.Dependencies;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Containers
{
    public abstract class DependencyContainerBase : MonoBehaviour, IDependencyContainer
    {
        private ConfigurableDependencyContainer _innerContainer;

        private ConfigurableDependencyContainer InnerContainer => _innerContainer ??
                                                                  (_innerContainer =
                                                                      new ConfigurableDependencyContainer(
                                                                          builder => ComposeDependencies(builder)
                                                                      ));

        /// <inheritdoc />
        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return InnerContainer.TryResolve(type, out dependency);
        }

        /// <inheritdoc />
        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return InnerContainer.CanBeResolvedSafe(type);
        }

        /// <inheritdoc />
        public void GetAllRegisteredExternalObjects(ICollection<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            InnerContainer.GetAllRegisteredExternalObjects(objects);
        }

        /// <inheritdoc />
        public void GetAllRegisteredObjectsOfType<T>(ICollection<T> objects) where T : class
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            InnerContainer.GetAllRegisteredObjectsOfType(objects);
        }

        /// <summary>
        ///     Check dependency graph for loops.
        /// </summary>
        /// <returns>True if there is a loop, false otherwise.</returns>
        public bool HasLoops() => InnerContainer.HasLoops();

        internal bool DependenciesCanBeResolved(
            [NotNull] List<(Type dependent, Type unresolvedDependency)> unresolvedDependencies) =>
            InnerContainer.DependenciesCanBeResolved(unresolvedDependencies);

        internal DependencyWithMetadata[] GetRawDependencies() => InnerContainer.GetRawDependencies();

        protected abstract void ComposeDependencies(ICanRegisterContainerBuilder builder);
    }
}