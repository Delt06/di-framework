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

        private void Awake()
        {
            Resolve();
        }

        public void Resolve()
        {
            if (_resolved) return;

            _resolved = true;
            var resolver = ResolverPool.Rent(this, _dependencySource);
            resolver.Resolve();
            ResolverPool.Return(resolver);

            if (_destroyWhenFinished)
                Destroy(this);
        }

        public bool CanBeResolvedSafe(MonoBehaviour component, Type type)
        {
            var resolver = ResolverPool.Rent(this, _dependencySource);
            var canBeResolvedSafe = resolver.CanBeResolvedSafe(component, type);
            ResolverPool.Return(resolver);
            return canBeResolvedSafe;
        }

        void IInitializable.EnsureInitialized() => Resolve();

        private bool _resolved;
    }
}