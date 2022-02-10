using System;
using System.Runtime.CompilerServices;
using DELTation.DIFramework.Editor;
using DELTation.DIFramework.Resolution;
using UnityEngine;

namespace DELTation.DIFramework
{
    [DisallowMultipleComponent]
    public sealed class Resolver : MonoBehaviour, IInitializable, IResolver, IShowIconInHierarchy
    {
        [SerializeField, HideInInspector] private bool _overrideDependencySource;
        [SerializeField, HideInInspector] private DependencySource _dependencySource = DependencySources.All;
        [SerializeField, HideInInspector] private bool _destroyWhenFinished = true;

        private bool _resolved;

        private void Awake()
        {
            Resolve();
        }

        void IInitializable.EnsureInitialized() => Resolve();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resolve()
        {
            if (_resolved) return;

            _resolved = true;

            var resolver = RentResolver();
            resolver.Resolve();
            ResolverPool.Return(resolver);

            if (_destroyWhenFinished)
                Destroy(this);
        }

        public bool CanBeResolvedSafe(MonoBehaviour component, Type type, out DependencySource actualSource)
        {
            var resolver = RentResolver();
            var canBeResolvedSafe = resolver.CanBeResolvedSafe(component, type, out actualSource);
            ResolverPool.Return(resolver);
            return canBeResolvedSafe;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CachedComponentResolver RentResolver()
        {
            var defaultDependencySource = DiSettings.TryGetInstance(out var settings)
                ? settings.DefaultDependencySource
                : DependencySources.All;
            var dependencySource = _overrideDependencySource ? _dependencySource : defaultDependencySource;
            return ResolverPool.Rent(gameObject, dependencySource);
        }
    }
}