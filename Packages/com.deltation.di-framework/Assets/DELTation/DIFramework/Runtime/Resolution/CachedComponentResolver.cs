﻿using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Baking;
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
            _resolutionFunction = Resolve;
            _isAffectedExtraCondition = IsAffectedExtraCondition;
        }

        public bool CabBeResolvedSafe(MonoBehaviour component, Type type)
        {
            var context = new ResolutionContext(_resolverComponent, component);
            return _dependencySource.CanBeResolvedSafe(context, type);
        }

        public void Resolve()
        {
            _affectedComponents.Clear();
            Injection.GetAffectedComponents(_affectedComponents, _resolverComponent.transform, _isAffectedExtraCondition
            );

            foreach (var (component, _) in _affectedComponents)
            {
                Inject(component);
            }

            _cache.Clear();
        }

        private static bool IsAffectedExtraCondition(MonoBehaviour mb) => BakedInjection.IsBaked(mb.GetType());

        private void Inject(MonoBehaviour component)
        {
            if (BakedInjection.TryInject(component, _resolutionFunction)) return;

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
                arguments[index] = Resolve(component, parameter.ParameterType);
            }

            method.Invoke(component, arguments);

            for (var index = 0; index < arguments.Length; index++)
            {
                arguments[index] = null;
            }

            Injection.ReturnArgumentsArray(arguments);
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

        private readonly ResolutionFunction _resolutionFunction;
        private readonly Func<MonoBehaviour, bool> _isAffectedExtraCondition;
        private readonly MonoBehaviour _resolverComponent;
        private readonly DependencySource _dependencySource;
        private readonly TypedCache _cache = new TypedCache();

        private readonly List<(MonoBehaviour component, int depth)> _affectedComponents =
            new List<(MonoBehaviour, int depth)>();
    }
}