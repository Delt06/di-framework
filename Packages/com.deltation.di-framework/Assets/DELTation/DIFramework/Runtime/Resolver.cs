using System;
using DELTation.DIFramework.Editor;
using DELTation.DIFramework.Resolution;
using UnityEngine;

namespace DELTation.DIFramework
{
    [DisallowMultipleComponent]
    public sealed class Resolver : MonoBehaviour, IInitializable, IResolver, IShowIconInHierarchy
    {
        [SerializeField] private DependencySource _dependencySource = DependencySources.All;
        [SerializeField] private bool _destroyWhenFinished = true;

        private void Awake()
        {
            Resolve();
        }

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

        private CachedComponentResolver RentResolver() => ResolverPool.Rent(this, _dependencySource);

        void IInitializable.EnsureInitialized() => Resolve();

        private bool _resolved;
    }
}