using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    public static class BakedInjection
    {
        public static bool DataExists = false;

        public static bool TryInject([NotNull] MonoBehaviour component, [NotNull] ResolutionFunction resolutionFunction)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));

            if (!BakedInjectionFunctions.TryGetValue(component.GetType(), out var injectionFunction))
                return false;

            injectionFunction(component, resolutionFunction);
            return true;
        }

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

        public static void Bake([NotNull] Type type, [NotNull] InjectionFunction injectionFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            BakedInjectionFunctions[type] =
                injectionFunction ?? throw new ArgumentNullException(nameof(injectionFunction));
        }

        public static void Bake([NotNull] Type type, [NotNull] PocoInstantiationFunction instantiationFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type} is abstract and thus cannot be instantiated.");
            BakedPocoInstantiationFunctions[type] = instantiationFunction ??
                                                    throw new ArgumentNullException(nameof(instantiationFunction));
        }

        public static void Clear()
        {
            BakedInjectionFunctions.Clear();
            BakedPocoInstantiationFunctions.Clear();
        }

        public static bool IsBaked([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return BakedInjectionFunctions.TryGetValue(type, out _);
        }

        public static int BakedInjectionFunctionsCount => BakedInjectionFunctions.Count;
        public static int BakedPocoInstantiationFunctionsCount => BakedPocoInstantiationFunctions.Count;

        private static readonly Dictionary<Type, InjectionFunction> BakedInjectionFunctions =
            new Dictionary<Type, InjectionFunction>();

        private static readonly Dictionary<Type, PocoInstantiationFunction> BakedPocoInstantiationFunctions =
            new Dictionary<Type, PocoInstantiationFunction>();
    }
}