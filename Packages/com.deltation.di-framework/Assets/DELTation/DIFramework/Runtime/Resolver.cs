using System;
using System.Collections.Generic;
using DELTation.DIFramework.Cache;
using DELTation.DIFramework.Editor;
using DELTation.DIFramework.Pooling;
using DELTation.DIFramework.Resolution;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework
{
    [DisallowMultipleComponent]
    public sealed class Resolver : MonoBehaviour, IInitializable, IResolver, IShowIconInHierarchy
    {
        [SerializeField] private DependencySource _dependencySource = DependencySources.All;
        [SerializeField] private bool _destroyWhenFinished = true;

        [SerializeField, HideInInspector] private List<Object> _cachedComponents = new List<Object>();
        [SerializeField, HideInInspector]
        private List<GameObjectWithUnityObject> _cachedLocalComponents = new List<GameObjectWithUnityObject>();

        [SerializeField, HideInInspector]
        private List<MonoBehaviour> _affectedComponentsCache = new List<MonoBehaviour>();

        public IReadOnlyList<Object> CachedComponents => _cachedComponents ?? (_cachedComponents = new List<Object>());

        public IReadOnlyList<GameObjectWithUnityObject> CachedLocalComponents => _cachedLocalComponents ??
            (_cachedLocalComponents =
                new List<GameObjectWithUnityObject>());

        public IReadOnlyList<MonoBehaviour> AffectedComponentsCache => _affectedComponentsCache;

        private void Awake()
        {
            Resolve();
        }

        public void Resolve()
        {
            if (_resolved) return;

            _resolved = true;

            var resolver = RentResolver();
            RemoveNullsFromCache();
            resolver.InitializeCacheFrom(_cachedComponents);
            if (_affectedComponentsCache.Count > 0)
                resolver.AffectedComponentsOverride = AffectedComponentsCache;
            resolver.Resolve();
            ResolverPool.Return(resolver);

            if (_destroyWhenFinished)
                Destroy(this);
        }

        private void RemoveNullsFromCache()
        {
            EnsureCacheIsNotNull();

            for (var index = _cachedComponents.Count - 1; index >= 0; index--)
            {
                if (_cachedComponents[index] == null)
                    _cachedComponents.RemoveAt(index);
            }

            for (var index = _cachedLocalComponents.Count - 1; index >= 0; index--)
            {
                if (_cachedLocalComponents[index].Object == null)
                    _cachedLocalComponents.RemoveAt(index);
            }

            for (var index = _affectedComponentsCache.Count - 1; index >= 0; index--)
            {
                if (_affectedComponentsCache[index] == null)
                    _affectedComponentsCache.RemoveAt(index);
            }
        }

        public void PopulateCache()
        {
            ClearCacheInternal();
            PopulateDependenciesCache();
            PopulateAffectedComponentsCache();

            Debug.Log($"Populate cache of {this}.", this);
        }

        private void PopulateDependenciesCache()
        {
            var resolver = RentResolver();
            resolver.DependencySource &= ~DependencySource.Global;
            resolver.CacheOnly = true;

            var localCacheDestination = ListPool<GameObjectWithObject>.Rent();
            resolver.ResolveAndDumpCache(_cachedComponents, localCacheDestination);

            foreach (var gameObjectWithObject in localCacheDestination)
            {
                if (gameObjectWithObject.Object is Object unityObject)
                {
                    var gameObjectWithUnityObject =
                        new GameObjectWithUnityObject(gameObjectWithObject.GameObject, unityObject);
                    _cachedLocalComponents.Add(gameObjectWithUnityObject);
                }
            }

            ListPool<GameObjectWithObject>.Return(localCacheDestination);
            ResolverPool.Return(resolver);
        }

        private void PopulateAffectedComponentsCache()
        {
            var affectedComponents = ListPool<(MonoBehaviour component, int depth)>.Rent();

            Injection.GetAffectedComponents(affectedComponents, transform);

            foreach (var (affectedComponent, _) in affectedComponents)
            {
                _affectedComponentsCache.Add(affectedComponent);
            }

            ListPool<(MonoBehaviour component, int depth)>.Return(affectedComponents);
        }

        public void ClearCache()
        {
            ClearCacheInternal();
            Debug.Log($"Cleared the cache of {this}.", this);
        }

        private void ClearCacheInternal()
        {
            EnsureCacheIsNotNull();
            _cachedComponents.Clear();
            _cachedLocalComponents.Clear();
            _affectedComponentsCache.Clear();
        }

        private void EnsureCacheIsNotNull()
        {
            if (_cachedComponents == null)
                _cachedComponents = new List<Object>();
            if (_cachedLocalComponents == null)
                _cachedLocalComponents = new List<GameObjectWithUnityObject>();
            if (_affectedComponentsCache == null)
                _affectedComponentsCache = new List<MonoBehaviour>();
        }

        public bool CanBeResolvedSafe(MonoBehaviour component, Type type, out DependencySource actualSource)
        {
            var resolver = RentResolver();
            var canBeResolvedSafe = resolver.CanBeResolvedSafe(component, type, out actualSource);
            ResolverPool.Return(resolver);
            return canBeResolvedSafe;
        }

        private CachedComponentResolver RentResolver() => ResolverPool.Rent(gameObject, _dependencySource);

        void IInitializable.EnsureInitialized() => Resolve();

        private bool _resolved;
    }
}