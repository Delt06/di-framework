using System;
using System.Collections.Generic;
using DELTation.DIFramework.Dependencies;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;
using static DELTation.DIFramework.ContainersExtensions;

namespace DELTation.DIFramework.Containers
{
    internal sealed class ConfigurableDependencyContainer : IDependencyContainer
    {
        private readonly TypedCache _cache = new TypedCache();
        private readonly Action<ContainerBuilder> _composeDependencies;
        private readonly PocoInjection.ResolutionFunction _containerBuilderResolutionFunction;
        private readonly Predicate<object> _isExternal;
        private readonly TagCollection<object> _objectTags = new TagCollection<object>();
        private bool _initialized;

        public ConfigurableDependencyContainer([NotNull] Action<ContainerBuilder> composeDependencies)
        {
            _composeDependencies = composeDependencies ?? throw new ArgumentNullException(nameof(composeDependencies));
            _containerBuilderResolutionFunction = TryResolveAllowingInternal;
            _isExternal = o => !IsInternal(o);
        }

        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var builder = CreateContainerBuilder();
            _composeDependencies(builder);

            for (var i = 0; i < builder.DependenciesCount; i++)
            {
                if (builder.GetTags(i).Contains(typeof(InternalOnlyTag))) continue;
                if (ConformsTo(builder, i, type))
                    return true;
            }

            return false;
        }

        public void GetAllRegisteredExternalObjects(ICollection<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            EnsureInitialized();
            _cache.AddObjectsTo(objects, _isExternal);
        }

        public void GetAllRegisteredObjectsOfType<T>(ICollection<T> objects) where T : class
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            EnsureInitialized();
            _cache.AddAllObjectsOfTypeTo(objects);
        }

        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            EnsureInitialized();
            if (!_cache.TryGet(type, out dependency)) return false;
            if (!IsInternal(dependency)) return true;
            dependency = default;
            return false;
        }

        private bool IsInternal(object obj) => _objectTags.HasTag(obj, typeof(InternalOnlyTag));

        private ContainerBuilder CreateContainerBuilder() => new ContainerBuilder(_containerBuilderResolutionFunction);

        private bool TryResolveAllowingInternal(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            EnsureInitialized();
            return _cache.TryGet(type, out dependency);
        }

        /// <summary>
        ///     Check dependency graph for loops.
        /// </summary>
        /// <returns>True if there is a loop, false otherwise.</returns>
        public bool HasLoops()
        {
            var builder = CreateContainerBuilder();
            _composeDependencies(builder);
            return !builder.TrySortTopologically(true);
        }

        internal bool DependenciesCanBeResolved(
            [NotNull] List<(Type dependent, Type unresolvedDependency)> unresolvedDependencies)
        {
            var builder = CreateContainerBuilder();
            _composeDependencies(builder);
            builder.TrySortTopologically();
            return builder.DependenciesCanBeResolved(unresolvedDependencies);
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;

            _initialized = true;

            var builder = CreateContainerBuilder();
            _composeDependencies(builder);
            if (!builder.TrySortTopologically())
                throw new DependencyLoopException();

            for (var index = 0; index < builder.DependenciesCount; index++)
            {
                var @object = builder.GetOrCreateObject(index);
                Register(@object);

                var tags = builder.GetTags(index);
                _objectTags.AddMany(@object, tags);
            }
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

        public DependencyWithMetadata[] GetRawDependencies()
        {
            var builder = CreateContainerBuilder();
            _composeDependencies(builder);
            builder.TrySortTopologically();
            return builder.GetDependencies();
        }

        private class TagCollection<TKey>
        {
            private readonly Dictionary<TKey, HashSet<Type>> _tags = new Dictionary<TKey, HashSet<Type>>();

            internal void AddMany(TKey key, HashSet<Type> tags)
            {
                foreach (var tag in tags)
                {
                    AddTag(key, tag);
                }
            }

            public bool HasTag(TKey key, Type tag)
            {
                if (!_tags.TryGetValue(key, out var tags)) return false;
                return tags != null && tags.Contains(tag);
            }

            private void AddTag(TKey key, Type tag)
            {
                if (!_tags.TryGetValue(key, out var tags))
                    _tags[key] = tags = new HashSet<Type>();
                tags.Add(tag);
            }
        }
    }
}