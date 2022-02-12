using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Baking;
using DELTation.DIFramework.Exceptions;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Resolution
{
    public static class PocoInjection
    {
        public delegate bool ResolutionFunction([NotNull] Type type, out object @object);

        private static readonly IDictionary<Type, IReadOnlyList<ParameterInfo>> ParametersCache =
            new Dictionary<Type, IReadOnlyList<ParameterInfo>>();

        public static bool IsInjectable([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return TryGetInjectableConstructorParameters(type, out _);
        }

        public static bool IsPoco([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return !typeof(Object).IsAssignableFrom(type);
        }

        public static bool TryGetInjectableConstructorParameters([NotNull] Type type,
            out IReadOnlyList<ParameterInfo> parameters)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (ParametersCache.TryGetValue(type, out parameters))
                return parameters != null;

            if (TryGetInjectableConstructorParametersInternal(type, out parameters))
                ParametersCache[type] = parameters;
            else
                ParametersCache[type] = null;

            return parameters != null;
        }

        private static bool TryGetInjectableConstructorParametersInternal([NotNull] Type type,
            out IReadOnlyList<ParameterInfo> parameters)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            IReadOnlyList<ParameterInfo> foundParameters = null;

            foreach (var constructor in constructors)
            {
                parameters = constructor.GetParameters();
                if (!parameters.AreInjectable()) continue;

                if (foundParameters == null)
                    foundParameters = parameters;
                else
                    return false;
            }

            parameters = foundParameters;
            return foundParameters != null;
        }

        internal static object CreateInstance([NotNull] Type type, [NotNull] ResolutionFunction resolutionFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));

            if (TryInstantiateViaBakedData(type, resolutionFunction, out var bakedDataInstance))
                return bakedDataInstance;

            if (DiSettings.TryGetInstance(out var settings) && settings.UseBakedData)
            {
                object PocoResolutionFunction(Type resolvedType)
                {
                    if (resolutionFunction(resolvedType, out var resolvedObject)) return resolvedObject;
                    throw new DependencyNotRegisteredException(resolvedType);
                }

                if (BakedInjection.TryInstantiate(type, PocoResolutionFunction, out var bakedInstance))
                    return bakedInstance;
            }

            if (!TryGetInjectableConstructorParameters(type, out var parameters))
                throw new ArgumentException($"Type {type} does not have an injectable constructor.");

            var arguments = Injection.RentArgumentsArray(parameters.Count);

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameterType = parameters[index].ParameterType;
                if (resolutionFunction(parameterType, out var dependency))
                {
                    arguments[index] = dependency;
                }
                else
                {
                    Injection.ReturnArgumentsArray(arguments);
                    throw new DependencyNotRegisteredException(parameterType);
                }
            }

            var instance = Activator.CreateInstance(type, arguments);
            Injection.ReturnArgumentsArray(arguments);
            return instance;
        }

        private static bool TryInstantiateViaBakedData([NotNull] Type type,
            [NotNull] ResolutionFunction resolutionFunction, out object instance)
        {
            if (DiSettings.TryGetInstance(out var settings) && settings.UseBakedData)
            {
                object PocoResolutionFunction(Type resolvedType)
                {
                    if (resolutionFunction(resolvedType, out var resolvedObject)) return resolvedObject;
                    throw new DependencyNotRegisteredException(resolvedType);
                }

                if (BakedInjection.TryInstantiate(type, PocoResolutionFunction, out instance))
                    return true;
            }

            instance = default;
            return false;
        }
    }
}