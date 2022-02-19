using System;
using System.Collections.Generic;
using System.Linq;
using DELTation.DIFramework.Pooling;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    internal static class DependencyUtils
    {
        public static bool DependenciesCanBeResolved<T>(T dependent,
            List<Type> possibleDependencyResolvers,
            ICollection<Type> unresolvedDependencies) where T : IDependency
        {
            var dependencies = ListPool<Type>.Rent();

            dependent.GetDependencies(dependencies);
            var dependenciesCanBeResolved =
                DependenciesCanBeResolved(dependencies, possibleDependencyResolvers, unresolvedDependencies);

            ListPool<Type>.Return(dependencies);
            return dependenciesCanBeResolved;
        }

        private static bool DependenciesCanBeResolved(IEnumerable<Type> dependencies,
            IReadOnlyCollection<Type> possibleDependencyResolvers, ICollection<Type> unresolvedDependencies)
        {
            bool CanBeResolved(Type dependency) =>
                possibleDependencyResolvers.Any(dependency.IsAssignableFrom);

            var canResolveAll = true;

            foreach (var dependency in dependencies)
            {
                if (CanBeResolved(dependency)) continue;

                canResolveAll = false;
                unresolvedDependencies.Add(dependency);
            }

            return canResolveAll;
        }

        public static bool DependsOn<T1, T2>(T1 dependent, T2 dependency)
            where T1 : IDependency where T2 : IDependency =>
            DependsOn(dependent, dependency, out _);

        public static bool DependsOn<T1, T2>(T1 dependent, T2 dependency, out Type requiredDependencyType)
            where T1 : IDependency where T2 : IDependency
        {
            var dependents = ListPool<Type>.Rent();

            dependent.GetDependencies(dependents);
            var dependencyType = dependency.GetResultingType();
            var dependsOn = DependsOn(dependents, dependencyType, out requiredDependencyType);

            ListPool<Type>.Return(dependents);
            return dependsOn;
        }

        private static bool DependsOn([NotNull] IReadOnlyList<Type> dependents, [NotNull] Type dependency,
            out Type requiredDependencyType)
        {
            if (dependents == null) throw new ArgumentNullException(nameof(dependents));
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < dependents.Count; index++)
            {
                var dependent = dependents[index];
                if (dependent.IsAssignableFrom(dependency))
                {
                    requiredDependencyType = dependent;
                    return true;
                }
            }

            requiredDependencyType = default;
            return false;
        }
    }
}