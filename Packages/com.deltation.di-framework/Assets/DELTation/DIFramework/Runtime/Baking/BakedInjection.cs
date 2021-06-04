using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    /// <summary>
    /// Hub to define baked injection and instantiation functions.
    /// Warning: for internal usage only.
    /// </summary>
    public static class BakedInjection
    {
        /// <summary>
        /// Is there baked data?
        /// </summary>
        public static bool DataExists = false;

        /// <summary>
        /// Try inject using baked data.
        /// </summary>
        /// <param name="component">Component to inject dependencies to.</param>
        /// <param name="resolutionFunction">Resolution provider.</param>
        /// <returns>True if baked and injected, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If any of arguments are null.</exception>
        public static bool TryInject([NotNull] MonoBehaviour component, [NotNull] ResolutionFunction resolutionFunction)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));

            if (!BakedInjectionFunctions.TryGetValue(component.GetType(), out var injectionFunction))
                return false;

            injectionFunction(component, resolutionFunction);
            return true;
        }

        /// <summary>
        /// Try create an instance of the provided type using baked data. 
        /// </summary>
        /// <param name="type">Instantiated type.</param>
        /// <param name="resolutionFunction">Resolution provider.</param>
        /// <param name="instance">Created instance.</param>
        /// <returns>True if baked and instantiated, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If any of arguments are null.</exception>
        public static bool TryInstantiate([NotNull] Type type, [NotNull] PocoResolutionFunction resolutionFunction,
            out object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));

            if (!BakedPocoInstantiationFunctions.TryGetValue(type, out var pocoInstantiationFunction))
            {
                instance = default;
                return false;
            }

            instance = pocoInstantiationFunction(resolutionFunction);
            return true;
        }

        /// <summary>
        /// Define a baked injection function.
        /// </summary>
        /// <param name="type">Type that can be injected with the function.</param>
        /// <param name="injectionFunction">Injection procedure.</param>
        /// <exception cref="ArgumentNullException">If any of arguments are null.</exception>
        public static void Bake([NotNull] Type type, [NotNull] InjectionFunction injectionFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            BakedInjectionFunctions[type] =
                injectionFunction ?? throw new ArgumentNullException(nameof(injectionFunction));
        }

        /// <summary>
        /// Define a baked instantiation function.
        /// </summary>
        /// <param name="type">Type that can be created with the function.</param>
        /// <param name="instantiationFunction">Creation procedure.</param>
        /// <exception cref="ArgumentNullException">If any of arguments are null.</exception>
        /// <exception cref="ArgumentException">If type is abstract.</exception>
        public static void Bake([NotNull] Type type, [NotNull] PocoInstantiationFunction instantiationFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type} is abstract and thus cannot be instantiated.");
            BakedPocoInstantiationFunctions[type] = instantiationFunction ??
                                                    throw new ArgumentNullException(nameof(instantiationFunction));
        }

        /// <summary>
        /// Clear defined baked data.s
        /// </summary>
        public static void Clear()
        {
            BakedInjectionFunctions.Clear();
            BakedPocoInstantiationFunctions.Clear();
        }

        /// <summary>
        /// Check if injection is baked for the type.
        /// </summary>
        /// <param name="type">Checked type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
        public static bool IsInjectionBaked([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return BakedInjectionFunctions.TryGetValue(type, out _);
        }

        /// <summary>
        /// The number of defined baked injection functions.
        /// </summary>
        public static int BakedInjectionFunctionsCount => BakedInjectionFunctions.Count;

        /// <summary>
        /// The number of defined POCO instantiation functions.
        /// </summary>
        public static int BakedPocoInstantiationFunctionsCount => BakedPocoInstantiationFunctions.Count;

        private static readonly Dictionary<Type, InjectionFunction> BakedInjectionFunctions =
            new Dictionary<Type, InjectionFunction>();

        private static readonly Dictionary<Type, PocoInstantiationFunction> BakedPocoInstantiationFunctions =
            new Dictionary<Type, PocoInstantiationFunction>();
    }
}