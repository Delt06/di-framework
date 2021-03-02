using System;
using DELTation.DIFramework.Containers;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework
{
    public static class ContainersExtensions
    {
        public static bool ShouldBeIgnoredByContainer(this object obj) =>
            obj is IIgnoreByContainer || obj is IDependencyContainer ||
            obj is Component c && c.TryGetComponent(out IIgnoreByContainer _);

        public static bool TryResolve<T>([NotNull] this IDependencyContainer container, out T component) where T : class
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            if (container.TryResolve(typeof(T), out var resolvedComponent))
            {
                component = (T) resolvedComponent;
                return true;
            }

            component = default;
            return false;
        }
    }
}