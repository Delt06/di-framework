using System;
using DELTation.DIFramework.Resolution;
using UnityEngine;

namespace DELTation.DIFramework
{
    [DisallowMultipleComponent]
    public sealed class Resolver : MonoBehaviour, IInitializable, IResolver
    {
        [SerializeField] private DependencySource _dependencySource = DependencySources.All;
        [SerializeField] private bool _destroyWhenFinished = true;

        [HideInInspector] public bool UseBakedData = true;

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

        public bool CanBeResolvedSafe(MonoBehaviour component, Type type)
        {
            var resolver = RentResolver();
            var canBeResolvedSafe = resolver.CanBeResolvedSafe(component, type);
            ResolverPool.Return(resolver);
            return canBeResolvedSafe;
        }

        private CachedComponentResolver RentResolver() => ResolverPool.Rent(this, _dependencySource, UseBakedData);

        void IInitializable.EnsureInitialized() => Resolve();

        private bool _resolved;
    }
}