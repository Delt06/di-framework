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
            InnerResolver.Resolve();

            if (_destroyWhenFinished)
                Destroy(this);
        }

        public bool CabBeResolvedSafe(MonoBehaviour component, Type type) =>
            InnerResolver.CabBeResolvedSafe(component, type);

        private IResolver InnerResolver =>
            _implementation ?? (_implementation = new CachedComponentResolver(this, _dependencySource));

        void IInitializable.EnsureInitialized() => Resolve();

        private IResolver _implementation;
        private bool _resolved;
    }
}