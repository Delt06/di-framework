﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using DELTation.DIFramework.Pooling;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    /// <summary>
    ///     Contains methods related to injection of dependencies.
    /// </summary>
    public static class Injection
    {
        /// <summary>
        ///     The name of injected methods.
        /// </summary>
        public static readonly string Constructor = "Construct";

        private static readonly IDictionary<Type, List<MethodInfo>>
            ConstructMethods = new Dictionary<Type, List<MethodInfo>>();

        private static readonly IDictionary<MethodInfo, ParameterInfo[]> InjectableParameters =
            new Dictionary<MethodInfo, ParameterInfo[]>();

        private static readonly IDictionary<MethodInfo, ParameterInfo[]> Parameters =
            new Dictionary<MethodInfo, ParameterInfo[]>();

        private static readonly IDictionary<int, List<object[]>> FreeArgumentsArraysCache =
            new Dictionary<int, List<object[]>>();

        public static bool CacheIsEmpty() =>
            ConstructMethods.Count == 0 && InjectableParameters.Count == 0 && Parameters.Count == 0 &&
            FreeArgumentsArraysCache.Count == 0;

        /// <summary>
        ///     Invalidates injection cache.
        /// </summary>
        public static void InvalidateCache()
        {
            InjectableParameters.Clear();
            ConstructMethods.Clear();
            Parameters.Clear();
            FreeArgumentsArraysCache.Clear();
        }

        /// <summary>
        ///     Generates cache to improve further performance.
        /// </summary>
        /// <param name="gameObject">GameObject to get optimized components from.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="gameObject" /> is null.</exception>
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
        ///     Generates cache to improve further performance.
        /// </summary>
        /// <param name="types">Types to optimize.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="types" /> are null.</exception>
        public static void WarmUp([NotNull] params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
            {
                WarmUp(type);
            }
        }

        /// <summary>
        ///     Generates cache to improve further performance.
        /// </summary>
        /// <param name="type">Type to optimize.</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="type" /> is null.</exception>
        public static void WarmUp([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (var methodInfo in GetConstructMethods(type))
            {
                TryGetInjectableParameters(methodInfo, out _);
            }
        }

        /// <summary>
        ///     Returns all the dependencies of the type.
        /// </summary>
        /// <param name="componentType">Type to get dependencies of.</param>
        /// <returns>An IEnumerable of dependencies.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="componentType" /> is null.</exception>
        public static IEnumerable<Type> GetAllDependenciesOf([NotNull] Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            var constructMethods = GetConstructMethods(componentType);
            var parameterTypes = new HashSet<Type>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var ctorIndex = 0; ctorIndex < constructMethods.Count; ctorIndex++)
            {
                var constructMethod = constructMethods[ctorIndex];

                if (!Parameters.TryGetValue(constructMethod, out var parameters))
                    Parameters[constructMethod] = parameters = constructMethod.GetParameters();

                foreach (var parameter in parameters)
                {
                    parameterTypes.Add(parameter.ParameterType);
                }
            }

            return parameterTypes;
        }

        /// <summary>
        ///     Checks whether the type is injectable.
        /// </summary>
        /// <param name="componentType">Type to check.</param>
        /// <returns>True if type is injectable, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="componentType" /> is null.</exception>
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

        internal static bool AreInjectable([NotNull] this IReadOnlyList<ParameterInfo> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            for (var index = 0; index < parameters.Count; index++)
            {
                if (!parameters[index].IsInjectable())
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
            var tempComponentsList = ListPool<MonoBehaviour>.Rent();
            root.GetComponents(tempComponentsList);

            for (var index = 0; index < tempComponentsList.Count; index++)
            {
                var component = tempComponentsList[index];
                if (IsAffected(component, isAffectedExtraCondition))
                    affectedComponents.Add((component, depth));
            }

            ListPool<MonoBehaviour>.Return(tempComponentsList);

            for (var childIndex = 0; childIndex < root.childCount; childIndex++)
            {
                var child = root.GetChild(childIndex);
                if (child.TryGetComponent(out Resolver _)) continue;

                GetAffectedComponents(affectedComponents, child, isAffectedExtraCondition, depth + 1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryGetAffectedComponentsFast(List<(MonoBehaviour component, int depth)> affectedComponents,
            Transform root, [CanBeNull] Func<MonoBehaviour, bool> isAffectedExtraCondition = null)
        {
            var resolvers = ListPool<Resolver>.Rent();
            root.GetComponentsInChildren(resolvers);
            if (resolvers.Count > 1)
            {
                ListPool<Resolver>.Return(resolvers);
                return false;
            }

            ListPool<Resolver>.Return(resolvers);
            var components = ListPool<MonoBehaviour>.Rent();
            root.GetComponentsInChildren(true, components);

            for (var i = 0; i < components.Count; i++)
            {
                var component = components[i];
                if (IsAffected(component, isAffectedExtraCondition))
                    affectedComponents.Add((component, 0));
            }

            ListPool<MonoBehaviour>.Return(components);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAffected([CanBeNull] MonoBehaviour component,
            [CanBeNull] Func<MonoBehaviour, bool> isAffectedExtraCondition)
        {
            if (component is Resolver) return false;
            if (component == null) return false;

            var extraConditionIsMet = isAffectedExtraCondition != null && isAffectedExtraCondition(component);
            return extraConditionIsMet || HasAtLeastOneConstructor(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasAtLeastOneConstructor(MonoBehaviour component) =>
            GetConstructMethods(component.GetType()).Count > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            // Wipe the array to remove potential memory leaks
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = null;
            }

            var arraysList = GetArgumentsArraysList(array.Length);

            if (!arraysList.Contains(array))
                arraysList.Add(array);
        }

        private static List<object[]> GetArgumentsArraysList(int length)
        {
            if (FreeArgumentsArraysCache.TryGetValue(length, out var arraysList)) return arraysList;
            return FreeArgumentsArraysCache[length] = new List<object[]>();
        }
    }
}