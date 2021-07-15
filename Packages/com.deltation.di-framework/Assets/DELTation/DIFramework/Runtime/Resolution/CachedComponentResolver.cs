using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Baking;
using DELTation.DIFramework.Cache;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Pooling;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Resolution
{
    internal sealed class CachedComponentResolver : IResolver
    {
        public CachedComponentResolver(GameObject gameObject, DependencySource dependencySource)
        {
            GameObject = gameObject;
            DependencySource = dependencySource;
            _resolutionFunction = Resolve;
            _bakedIsAffectedExtraCondition = IsAffectedExtraCondition;
        }

        public GameObject GameObject { get; set; }
        public DependencySource DependencySource { get; set; }
        internal bool CacheOnly { get; set; }
        
        [CanBeNull]
        internal IReadOnlyList<MonoBehaviour> AffectedComponentsOverride { get; set; }

        public void Clear()
        {
            GameObject = default;
            _cache.Clear();
            _localCache.Clear();
            _affectedComponents.Clear();
            CacheOnly = false;
            AffectedComponentsOverride = null;
        }

        public bool CanBeResolvedSafe(MonoBehaviour component, Type type, out DependencySource actualSource)
        {
            var context = new ResolutionContext(GameObject, component);
            return DependencySource.CanBeResolvedSafe(context, type, out actualSource);
        }

        internal void InitializeCacheFrom([NotNull] IReadOnlyList<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            _cache.InitializeFrom(objects);
        }

        internal void InitializeLocalCacheFrom([NotNull] IReadOnlyList<GameObjectWithObject> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            _localCache.InitializeFrom(objects);
        }

        public void Resolve()
        {
            ResolveInternal();
            _cache.Clear();
            _localCache.Clear();
        }

        internal void ResolveAndDumpCache([CanBeNull] List<Object> dumpDestination = null,
            [CanBeNull] List<GameObjectWithObject> localDumpDestination = null)
        {
            ResolveInternal();

            if (dumpDestination != null) DumpCache(_cache, dumpDestination);

            if (localDumpDestination != null) DumpCache(_localCache, localDumpDestination);

            _cache.Clear();
            _localCache.Clear();
        }

        private static void DumpCache<TKey, TValue, TDestination>([NotNull] TypedCacheBase<TKey, TValue> cache,
            [NotNull] List<TDestination> dumpDestination)
        {
            var cachedObjects = ListPool<TValue>.Rent();
            cache.AddAllObjectsTo(cachedObjects);

            foreach (var cachedObject in cachedObjects)
            {
                if (cachedObject is TDestination destinationObject)
                    dumpDestination.Add(destinationObject);
            }

            ListPool<TValue>.Return(cachedObjects);
        }

        private void ResolveInternal()
        {
            if (TryGetAffectedComponentsOverride(out var affectedComponentsOverride))
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < affectedComponentsOverride.Count; i++)
                {
                    Inject(affectedComponentsOverride[i]);
                }
            }
            else
            {
                _affectedComponents.Clear();

                var extraCondition = UseBakedData ? _bakedIsAffectedExtraCondition : null;
                var transform = GameObject.transform;
                if (!Injection.TryGetAffectedComponentsFast(_affectedComponents, transform, extraCondition))
                    Injection.GetAffectedComponents(_affectedComponents, transform, extraCondition);
            

                foreach (var (component, _) in _affectedComponents)
                {
                    Inject(component);
                }
            }
        }

        private bool TryGetAffectedComponentsOverride(out IReadOnlyList<MonoBehaviour> affectedComponentsOverride)
        {
            affectedComponentsOverride = AffectedComponentsOverride;
            return affectedComponentsOverride != null;
        }

        private static bool UseBakedData => DiSettings.TryGetInstance(out var settings) &&
                                            settings.UseBakedData;

        private static bool IsAffectedExtraCondition(MonoBehaviour mb) => BakedInjection.IsInjectionBaked(mb.GetType());

        private void Inject(MonoBehaviour component)
        {
            if (UseBakedData && BakedInjection.TryInject(component, _resolutionFunction))
                return;

            var methods = Injection.GetConstructMethods(component.GetType());

            for (var index = 0; index < methods.Count; index++)
            {
                InjectThrough(component, methods[index]);
            }
        }

        private void InjectThrough(MonoBehaviour component, MethodInfo method)
        {
            if (!Injection.TryGetInjectableParameters(method, out var parameters))
                throw new NotInjectableException(component, method.Name);

            var arguments = Injection.RentArgumentsArray(parameters.Count);

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];

                var type = parameter.ParameterType;
                if (CacheOnly)
                    TryResolve(component, type, out _);
                else
                    arguments[index] = Resolve(component, type);
            }

            if (!CacheOnly)
                method.Invoke(component, arguments);

            for (var index = 0; index < arguments.Length; index++)
            {
                arguments[index] = null;
            }

            Injection.ReturnArgumentsArray(arguments);
        }

        private object Resolve(MonoBehaviour component, Type type)
        {
            if (TryResolve(component, type, out var dependency)) return dependency;

            throw new DependencyNotResolvedException(component, type);
        }

        private bool TryResolve(MonoBehaviour component, Type type, out object dependency)
        {
            if ((DependencySource & DependencySource.Local) != 0)
            {
                var gameObjectComponentType = new GameObjectComponentType(component.gameObject, type);
                if (_localCache.TryGet(gameObjectComponentType, out var gameObjectWithDependency))
                {
                    dependency = gameObjectWithDependency.Object;
                    return true;
                }
            }

            if (DependencySource != DependencySource.Local && DependencySource != 0)
                if (_cache.TryGet(type, out dependency))
                    return true;

            var context = new ResolutionContext(GameObject, component);
            if (DependencySource.TryResolve(context, type, out dependency, out var actualSource))
            {
                if (actualSource == DependencySource.Local)
                {
                    var obj = new GameObjectWithObject(component.gameObject, dependency);
                    _localCache.TryRegister(obj, out _);
                }
                else
                {
                    _cache.TryRegister(dependency, out _);
                }

                return true;
            }

            return false;
        }

        private readonly ResolutionFunction _resolutionFunction;
        private readonly Func<MonoBehaviour, bool> _bakedIsAffectedExtraCondition;
        private readonly TypedCache _cache = new TypedCache();
        private readonly LocalTypedCache _localCache = new LocalTypedCache();

        private readonly List<(MonoBehaviour component, int depth)> _affectedComponents =
            new List<(MonoBehaviour, int depth)>();
    }
}