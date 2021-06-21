using System;
using System.Collections.Generic;
using DELTation.DIFramework.Exceptions;
using JetBrains.Annotations;
using static DELTation.DIFramework.ContainersExtensions;

namespace DELTation.DIFramework.Containers
{
    public sealed class ConfigurableDependencyContainer : IDependencyContainer
    {
        private readonly Action<ContainerBuilder> _composeDependencies;

        public ConfigurableDependencyContainer([NotNull] Action<ContainerBuilder> composeDependencies) =>
            _composeDependencies = composeDependencies ?? throw new ArgumentNullException(nameof(composeDependencies));

        /// <summary>
        /// Check dependency graph for loops.
        /// </summary>
        /// <returns>True if there is a loop, false otherwise.</returns>
        public bool HasLoops()
        {
            var builder = new ContainerBuilder(TryResolve);
            _composeDependencies(builder);
            return !builder.TrySortTopologically(true);
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;

            _initialized = true;

            var builder = new ContainerBuilder(TryResolve);
            _composeDependencies(builder);
            if (!builder.TrySortTopologically())
                throw new DependencyLoopException();

            for (var index = 0; index < builder.DependenciesCount; index++)
            {
                var @object = builder.GetOrCreateObject(index);
                Register(@object);
            }
        }

        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var builder = new ContainerBuilder(TryResolve);
            _composeDependencies(builder);

            for (var i = 0; i < builder.DependenciesCount; i++)
            {
                if (ConformsTo(builder, i, type))
                    return true;
            }

            return false;
        }

        public void GetAllRegisteredObjects(ICollection<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            EnsureInitialized();
            _cache.AddAllObjectsTo(objects);
        }

        private static bool ConformsTo(ContainerBuilder builder, int index, Type checkedType) =>
            checkedType.IsAssignableFrom(builder.GetType(index));

        private void Register([NotNull] object dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (ShouldBeIgnoredByContainer(dependency)) return;

            if (_cache.TryRegister(dependency, out var registeredDependency))
                return;

            var type = dependency.GetType();
            throw new DependencyAlreadyRegistered(type, registeredDependency);
        }

        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            EnsureInitialized();
            return _cache.TryGet(type, out dependency);
        }

        private readonly TypedCache _cache = new TypedCache();
        private bool _initialized;
    }
}