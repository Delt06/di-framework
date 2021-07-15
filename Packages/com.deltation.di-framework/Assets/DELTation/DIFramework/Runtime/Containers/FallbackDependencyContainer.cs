using System;
using System.Collections.Generic;
using UnityEngine;
using static DELTation.DIFramework.ContainersExtensions;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Containers
{
    /// <summary>
    /// Container that searches for objects using FindObjectOfType.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Dependency Container/Fallback Dependency Container")]
    public sealed class FallbackDependencyContainer : MonoBehaviour, IDependencyContainer
    {
        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return TryFindObjectOfType(type, out _);
        }

        void IDependencyContainer.GetAllRegisteredObjects(ICollection<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
        }

        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_cache.TryGet(type, out dependency)) return true;
            if (!TryFindObjectOfType(type, out dependency)) return false;

            _cache.TryRegister(dependency, out _);
            return true;
        }

        private static bool TryFindObjectOfType(Type type, out object dependency)
        {
            if (!typeof(Object).IsAssignableFrom(type))
            {
                dependency = default;
                return false;
            }

            var dependencies = FindObjectsOfType(type);

            foreach (var d in dependencies)
            {
                if (d == null) continue;
                if (ShouldBeIgnoredByContainer(d)) continue;

                dependency = d;
                return true;
            }

            dependency = default;
            return false;
        }

        private readonly TypedCache _cache = new TypedCache();
    }
}