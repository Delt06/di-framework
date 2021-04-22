using System;
using System.ComponentModel;
using System.Linq;
using DELTation.DIFramework.Exceptions;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Containers
{
    public sealed class ConfigurableDependencyContainer : IDependencyContainer
    {
        private readonly Action<ContainerBuilder> _composeDependencies;

        public ConfigurableDependencyContainer([NotNull] Action<ContainerBuilder> composeDependencies) =>
            _composeDependencies = composeDependencies ?? throw new ArgumentNullException(nameof(composeDependencies));

        private void EnsureInitialized()
        {
            if (_initialized) return;

            _initialized = true;

            var builder = new ContainerBuilder(this);
            _composeDependencies(builder);
            builder.SortTopologically();

            for (var index = 0; index < builder.DependenciesCount; index++)
            {
                var @object = builder.GetOrCreateObject(index);
                Register(@object);
            }
        }

        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var builder = new ContainerBuilder(this);
            _composeDependencies(builder);
            return Enumerable.Range(0, builder.DependenciesCount)
                .Any(i => ConformsTo(builder, i, type));
        }

        private static bool ConformsTo(ContainerBuilder builder, int index, Type checkedType) =>
            checkedType.IsAssignableFrom(builder.GetType(index));

        private void Register([NotNull] object dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (dependency.ShouldBeIgnoredByContainer()) return;

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