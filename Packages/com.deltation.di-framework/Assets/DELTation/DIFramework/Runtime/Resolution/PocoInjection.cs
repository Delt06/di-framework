using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Exceptions;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Resolution
{
    internal static class PocoInjection
    {
        public static bool TryGetInjectableConstructorParameters([NotNull] Type type, out ParameterInfo[] parameters)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (Parameters.TryGetValue(type, out parameters))
                return parameters != null;

            if (TryGetInjectableConstructorParametersInternal(type, out parameters))
                Parameters[type] = parameters;
            else
                Parameters[type] = null;

            return parameters != null;
        }

        private static bool TryGetInjectableConstructorParametersInternal([NotNull] Type type,
            out ParameterInfo[] parameters)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ParameterInfo[] foundParameters = null;

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

            if (!TryGetInjectableConstructorParameters(type, out var parameters))
                throw new ArgumentException($"Type {type} does not have an injectable constructor.");

            var arguments = Injection.RentArgumentsArray(parameters.Length);

            for (var index = 0; index < parameters.Length; index++)
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

        internal delegate bool ResolutionFunction([NotNull] Type type, out object @object);

        private static readonly IDictionary<Type, ParameterInfo[]> Parameters = new Dictionary<Type, ParameterInfo[]>();
    }
}