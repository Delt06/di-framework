using System;
using System.Collections.Generic;
using System.Reflection;
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

        private static readonly IDictionary<Type, ParameterInfo[]> Parameters = new Dictionary<Type, ParameterInfo[]>();
    }
}