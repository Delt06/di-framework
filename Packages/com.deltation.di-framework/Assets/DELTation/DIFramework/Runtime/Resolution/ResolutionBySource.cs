using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Resolution
{
    internal static class ResolutionBySource
    {
        public static bool CanBeResolvedSafe(this DependencySource source, ResolutionContext context, Type type,
            out DependencySource actualSource) =>
            source.TryResolveInGameObject(context, type, out _, out actualSource) ||
            source.CanBeResolvedGloballySafe(type, out actualSource);

        public static bool TryResolve(this DependencySource source, ResolutionContext context, Type type,
            out object result,
            out DependencySource actualSource) =>
            source.TryResolveInGameObject(context, type, out result, out actualSource) ||
            source.TryResolveGlobally(type, out result, out actualSource);

        private static bool TryResolveInGameObject(this DependencySource source, ResolutionContext context, Type type,
            out object result, out DependencySource dependencySource)
        {
            if (CanBeResolvedInGameObject(type))
            {
                if (TryResolveLocally(source, context, type, out result))
                {
                    dependencySource = DependencySource.Local;
                    return true;
                }

                if (TryResolveInChildren(source, context, type, out result))
                {
                    dependencySource = DependencySource.Children;
                    return true;
                }

                if (TryResolveInParent(source, context, type, out result))
                {
                    dependencySource = DependencySource.Parent;
                    return true;
                }
            }

            result = default;
            dependencySource = default;
            return false;
        }

        private static bool CanBeResolvedInGameObject(Type type) =>
            typeof(Component).IsAssignableFrom(type) || type.IsInterface;

        private static bool TryResolveLocally(DependencySource source, ResolutionContext context, Type type,
            out object result)
        {
            if (source.Includes(DependencySource.Local) &&
                context.Component.TryGetComponent(type, out var foundComponent))
            {
                result = foundComponent;
                return true;
            }

            result = default;
            return false;
        }

        private static bool TryResolveInChildren(DependencySource source, ResolutionContext context, Type type,
            out object result)
        {
            if (source.Includes(DependencySource.Children))
            {
                var foundComponent = context.Resolver.GetComponentInChildren(type);
                if (foundComponent != null)
                {
                    result = foundComponent;
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static bool TryResolveInParent(DependencySource source, ResolutionContext context, Type type,
            out object result)
        {
            if (source.Includes(DependencySource.Parent))
            {
                var foundComponent = context.Resolver.GetComponentInParent(type);
                if (foundComponent != null)
                {
                    result = foundComponent;
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static bool TryResolveGlobally(this DependencySource source, Type type, out object result,
            out DependencySource dependencySource)
        {
            for (var index = RootDependencyContainer.InstancesCount - 1; index >= 0; index--)
            {
                var container = RootDependencyContainer.GetInstance(index);
                if (source.TryResolveGlobally(container, type, out result, out dependencySource))
                    return true;
            }

            result = default;
            dependencySource = default;
            return false;
        }

        private static bool TryResolveGlobally(this DependencySource source, RootDependencyContainer container,
            Type type, out object result,
            out DependencySource dependencySource)
        {
            if (source.Includes(DependencySource.Global) && container && container.TryResolve(type, out result))
            {
                dependencySource = DependencySource.Global;
                return true;
            }

            result = default;
            dependencySource = default;
            return false;
        }

        private static bool CanBeResolvedGloballySafe(this DependencySource source, Type type,
            out DependencySource actualSource)
        {
            if (!source.Includes(DependencySource.Global))
            {
                actualSource = default;
                return false;
            }

            var containers = Object.FindObjectsOfType<RootDependencyContainer>();
            var canBeResolved = containers.Any(container => container.CanBeResolvedSafe(type));
            actualSource = canBeResolved ? DependencySource.Global : default;
            return canBeResolved;
        }

        private static bool Includes(this DependencySource source, DependencySource other) => (source & other) != 0;
    }
}