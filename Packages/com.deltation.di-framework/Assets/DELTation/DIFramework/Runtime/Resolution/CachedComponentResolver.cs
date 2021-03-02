using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Exceptions;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal sealed class CachedComponentResolver : IResolver
    {
        public CachedComponentResolver(MonoBehaviour resolverComponent, DependencySource dependencySource)
        {
            _resolverComponent = resolverComponent;
            _dependencySource = dependencySource;
        }

        public bool CabBeResolvedSafe(MonoBehaviour component, Type type)
        {
            var context = new ResolutionContext(_resolverComponent, component);
            return _dependencySource.CanBeResolvedSafe(context, type);
        }

        public void Resolve()
        {
            _affectedComponents.Clear();
            Injection.GetAffectedComponents(_affectedComponents, _resolverComponent.transform);

            foreach (var (component, _) in _affectedComponents)
            {
                Inject(component);
            }

            _cache.Clear();
        }

        private void Inject(MonoBehaviour component)
        {
            var methods = Injection.GetSuitableMethodsIn(component.GetType());

            for (var index = 0; index < methods.Count; index++)
            {
                InjectThrough(component, methods[index]);
            }
        }

        private void InjectThrough(MonoBehaviour component, MethodInfo method)
        {
            if (!Injection.TryGetInjectableParameters(method, out var parameters))
                throw new NotInjectableException(component, method.Name);

            var arguments = Injection.GetArgumentsArray(parameters.Count);

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                arguments[index] = Resolve(component, parameter.ParameterType);
            }

            method.Invoke(component, arguments);

            for (var index = 0; index < arguments.Length; index++)
            {
                arguments[index] = null;
            }
        }

        private object Resolve(MonoBehaviour component, Type type)
        {
            if (_cache.TryGet(type, out var dependency)) return dependency;

            var context = new ResolutionContext(_resolverComponent, component);
            if (_dependencySource.TryResolve(context, type, out dependency, out var actualSource))
            {
                if (IsCacheable(actualSource))
                    _cache.TryRegister(dependency, out _);

                return dependency;
            }

            throw new DependencyNotResolvedException(type);
        }

        private static bool IsCacheable(DependencySource source) => source != DependencySource.Local;

        private readonly MonoBehaviour _resolverComponent;
        private readonly DependencySource _dependencySource;
        private readonly TypedCache _cache = new TypedCache();

        private readonly List<(MonoBehaviour component, int depth)> _affectedComponents =
            new List<(MonoBehaviour, int depth)>();
    }
}