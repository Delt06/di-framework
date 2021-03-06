﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DELTation.DIFramework.Resolution.Injection;

namespace DELTation.DIFramework.Reporting
{
    public sealed class ResolverReport
    {
        public int Resolved { get; private set; }
        public int NotResolved { get; private set; }
        public int NotInjectable { get; private set; }
        public IReadOnlyCollection<ComponentResolutionData> ComponentsData => _componentsData;

        public void Generate()
        {
            var components = GetAffectedComponents(_resolver.transform).ToArray();
            GenerateCounts(components);
            GenerateComponentsData(components);
        }

        private void GenerateCounts((MonoBehaviour component, int depth)[] components)
        {
            (Resolved, NotResolved, NotInjectable) =
                GetResolutionStatistics(_resolver, components.Select(c => c.component));
        }

        private static (int resolved, int notResolved, int notInjectable) GetResolutionStatistics(Resolver resolver,
            IEnumerable<MonoBehaviour> components)
        {
            var types = components
                .Select(component => (component, GetAllDependenciesOf(component.GetType())))
                .ToArray();

            var resolved = CountAndSum(types, (c, t) => CanBeResolved(resolver, t, c, out _));
            var notResolved = CountAndSum(types,
                (c, t) => IsInjectable(c.GetType()) && !CanBeResolved(resolver, t, c, out _)
            );
            var notInjectable = types.Count(t => !IsInjectable(t.component.GetType()));
            return (resolved, notResolved, notInjectable);
        }

        private static int CountAndSum(IEnumerable<(MonoBehaviour component, IEnumerable<Type> types)> dependencies,
            Func<MonoBehaviour, Type, bool> predicate)
        {
            return dependencies.Sum(d => d.types.Count(t => predicate(d.component, t)));
        }

        private void GenerateComponentsData((MonoBehaviour component, int depth)[] components)
        {
            _componentsData.Clear();

            foreach (var (component, depth) in components)
            {
                var dependencies = GetAllDependenciesOf(component.GetType()).ToArray();
                var injectable = IsInjectable(component.GetType());
                var resolvedDependencies = new List<(Type type, DependencySource? source)>();

                foreach (var dependency in dependencies)
                {
                    var canBeResolved = CanBeResolved(_resolver, dependency, component, out var source);
                    resolvedDependencies.Add((dependency, canBeResolved ? source : (DependencySource?) null));
                }

                var data = new ComponentResolutionData(component, depth, injectable, resolvedDependencies.ToArray());
                _componentsData.Add(data);
            }
        }

        private static bool CanBeResolved(Resolver resolver, Type dependency, MonoBehaviour component,
            out DependencySource source) =>
            resolver.CanBeResolvedSafe(component, dependency, out source);

        public ResolverReport(Resolver resolver) => _resolver = resolver;

        private readonly Resolver _resolver;
        private readonly List<ComponentResolutionData> _componentsData = new List<ComponentResolutionData>();
    }
}