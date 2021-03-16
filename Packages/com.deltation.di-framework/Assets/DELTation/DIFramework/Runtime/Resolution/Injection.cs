using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    /// <summary>
    /// Contains methods related to injection of dependencies.
    /// </summary>
    public static class Injection
    {
        /// <summary>
        /// Invalidates injection cache.
        /// </summary>
        public static void InvalidateCache()
        {
            InjectableParameters.Clear();
            ConstructMethods.Clear();
            FreeArgumentsArraysCache.Clear();
        }

        /// <summary>
        /// Generates cache to improve further performance.
        /// </summary>
        /// <param name="gameObject">GameObject to get optimized components from.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="gameObject"/> is null.</exception>
        public static void WarmUp([NotNull] GameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));

            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                WarmUp(component.GetType());
            }
        }

        /// <summary>
        /// Generates cache to improve further performance.
        /// </summary>
        /// <param name="types">Types to optimize.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="types"/> are null.</exception>
        public static void WarmUp([NotNull] params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
            {
                WarmUp(type);
            }
        }

        /// <summary>
        /// Generates cache to improve further performance.
        /// </summary>
        /// <param name="type">Type to optimize.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="type"/> is null.</exception>
        public static void WarmUp([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (var methodInfo in GetConstructMethods(type))
            {
                TryGetInjectableParameters(methodInfo, out _);
            }
        }

        /// <summary>
        /// Returns all the dependencies of the type.
        /// </summary>
        /// <param name="componentType">Type to get dependencies of.</param>
        /// <returns>An IEnumerable of dependencies.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="componentType"/> is null.</exception>
        public static IEnumerable<Type> GetAllDependenciesOf([NotNull] Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            return GetConstructMethods(componentType)
                .SelectMany(m => m.GetParameters())
                .Select(p => p.ParameterType)
                .Distinct();
        }

        /// <summary>
        /// Checks whether the type is injectable.
        /// </summary>
        /// <param name="componentType">Type to check.</param>
        /// <returns>True if type is injectable, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="componentType"/> is null.</exception>
        public static bool IsInjectable([NotNull] Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            var methods = GetConstructMethods(componentType);

            for (var i = 0; i < methods.Count; i++)
            {
                if (!TryGetInjectableParameters(methods[i], out _))
                    return false;
            }

            return true;
        }

        internal static bool AreInjectable([NotNull] this ParameterInfo[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            foreach (var parameter in parameters)
            {
                if (!parameter.IsInjectable())
                    return false;
            }

            return true;
        }

        private static bool IsInjectable([NotNull] this ParameterInfo parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            var parameterType = parameter.ParameterType;
            return !parameterType.IsValueType && !parameter.IsOut &&
                   !parameter.IsIn && !parameterType.IsByRef;
        }

        internal static void GetAffectedComponents(List<(MonoBehaviour component, int depth)> affectedComponents,
            Transform root, [CanBeNull] Func<MonoBehaviour, bool> isAffectedExtraCondition = null,
            int depth = 0)
        {
            var components = root.GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component is null) continue;
                if (component is Resolver) continue;

                var extraConditionIsMet = isAffectedExtraCondition != null && isAffectedExtraCondition(component);
                if (extraConditionIsMet || HasAtLeastOneConstructor(component))
                    affectedComponents.Add((component, depth));
            }

            foreach (Transform child in root)
            {
                if (child.TryGetComponent(out Resolver _)) continue;

                GetAffectedComponents(affectedComponents, child, isAffectedExtraCondition, depth + 1);
            }
        }

        private static bool HasAtLeastOneConstructor(MonoBehaviour component) =>
            GetConstructMethods(component.GetType()).Count > 0;

        public static IEnumerable<(MonoBehaviour component, int depth)> GetAffectedComponents(Transform root,
            [CanBeNull] Func<MonoBehaviour, bool> isAffectedExtraCondition = null, int depth = 0)
        {
            var components = new List<(MonoBehaviour, int)>();
            GetAffectedComponents(components, root, isAffectedExtraCondition, depth);
            return components;
        }

        public static IReadOnlyList<MethodInfo> GetConstructMethods(Type type)
        {
            if (ConstructMethods.TryGetValue(type, out var methods)) return methods;

            var suitableMethods = new List<MethodInfo>();

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsConstructMethod(method))
                    suitableMethods.Add(method);
            }

            return ConstructMethods[type] = suitableMethods;
        }

        public static bool TryGetInjectableParameters([NotNull] MethodInfo method,
            out IReadOnlyList<ParameterInfo> parameters)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            if (InjectableParameters.TryGetValue(method, out var parametersInfo))
            {
                parameters = parametersInfo;
                return true;
            }

            parametersInfo = method.GetParameters();
            if (parametersInfo.AreInjectable())
            {
                InjectableParameters[method] = parametersInfo;
                parameters = parametersInfo;
                return true;
            }

            parameters = default;
            return false;
        }

        private static bool IsConstructMethod(MethodInfo method) =>
            method.Name == Constructor &&
            method.IsPublic && method.ReturnType == typeof(void);

        /// <summary>
        /// The name of injected methods.
        /// </summary>
        public const string Constructor = "Construct";

        private static readonly IDictionary<Type, List<MethodInfo>>
            ConstructMethods = new Dictionary<Type, List<MethodInfo>>();

        private static readonly IDictionary<MethodInfo, ParameterInfo[]> InjectableParameters =
            new Dictionary<MethodInfo, ParameterInfo[]>();

        internal static object[] RentArgumentsArray(int length)
        {
            var arraysList = GetArgumentsArraysList(length);

            if (arraysList.Count == 0) return new object[length];

            var lastIndex = arraysList.Count - 1;
            var array = arraysList[lastIndex];
            arraysList.RemoveAt(lastIndex);
            return array;
        }

        internal static void ReturnArgumentsArray([NotNull] object[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            var arraysList = GetArgumentsArraysList(array.Length);

            if (!arraysList.Contains(array))
                arraysList.Add(array);
        }

        private static List<object[]> GetArgumentsArraysList(int length)
        {
            if (FreeArgumentsArraysCache.TryGetValue(length, out var arraysList)) return arraysList;
            return FreeArgumentsArraysCache[length] = new List<object[]>();
        }

        private static readonly IDictionary<int, List<object[]>> FreeArgumentsArraysCache =
            new Dictionary<int, List<object[]>>();
    }
}