using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
     public static class BakedInjection
    {
        public static bool TryInject([NotNull] MonoBehaviour component, [NotNull] ResolutionFunction resolutionFunction)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));
            
            if (!BakedInjectionFunctions.TryGetValue(component.GetType(), out var injectionFunction)) return false;
            injectionFunction(component, resolutionFunction);
            return true;
        }

        public static void Bake([NotNull] Type type, [NotNull] InjectionFunction injectionFunction)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            BakedInjectionFunctions[type] = injectionFunction ?? throw new ArgumentNullException(nameof(injectionFunction));
        }

        public static void Clear()
        {
            BakedInjectionFunctions.Clear();
        }

        private static readonly Dictionary<Type, InjectionFunction> BakedInjectionFunctions =
            new Dictionary<Type, InjectionFunction>();
    }
}