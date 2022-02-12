using System;
using System.Collections.Generic;
using DELTation.DIFramework.Dependencies;
using DELTation.DIFramework.Pooling;
using DELTation.DIFramework.Sorting;
using JetBrains.Annotations;
using UnityEngine;
using static DELTation.DIFramework.Resolution.PocoInjection;

namespace DELTation.DIFramework
{
    public interface IAnyOperationContainerBuilder : IRegisteredContainerBuilder { }

    /// <summary>
    ///     Allows to build a custom container.
    /// </summary>
    internal sealed partial class ContainerBuilder : IAnyOperationContainerBuilder
    {
        private readonly List<DependencyWithMetadata> _dependencies = new List<DependencyWithMetadata>();

        private readonly ResolutionFunction _resolutionFunction;

        internal ContainerBuilder([NotNull] ResolutionFunction resolutionFunction) => _resolutionFunction =
            resolutionFunction ?? throw new ArgumentNullException(nameof(resolutionFunction));

        private bool WasAbleToRegisterLast { get; set; }

        internal int DependenciesCount => _dependencies.Count;


        /// <inheritdoc />
        public IRegisteredContainerBuilder Register<[MeansImplicitUse] T>() where T : class
        {
            AddDependency(new TypeDependency(typeof(T)));
            return OnRegisteredLast();
        }

        /// <inheritdoc />
        public IRegisteredContainerBuilder Register(object obj)
        {
            var isNull = UnityUtils.IsNullOrUnityNull(obj);
            if (Application.isPlaying && isNull)
                throw new ArgumentNullException(nameof(obj));
            if (isNull) return OnDidNotRegisterLast();
            AddDependency(new ObjectDependency(obj));
            return OnRegisteredLast();
        }

        /// <inheritdoc />
        public IRegisteredContainerBuilder RegisterFromMethodAsDelegate(Delegate factoryMethod)
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            var dependency = new FactoryMethodDelegateDependency(factoryMethod);
            AddDependency(dependency);
            return OnRegisteredLast();
        }

        private void AddDependency(IDependency dependency)
        {
            _dependencies.Add(new DependencyWithMetadata(dependency));
        }

        internal IRegisteredContainerBuilder OnDidNotRegisterLast()
        {
            WasAbleToRegisterLast = false;
            return this;
        }

        private void AddTag(int index, Type tag)
        {
            _dependencies[index].Tags.Add(tag);
        }

        private IAnyOperationContainerBuilder OnRegisteredLast()
        {
            WasAbleToRegisterLast = true;
            return this;
        }

        internal bool DependenciesCanBeResolved(
            [NotNull] List<(Type dependent, Type unresolvedDependency)> unresolvedDependencies)
        {
            if (unresolvedDependencies == null) throw new ArgumentNullException(nameof(unresolvedDependencies));

            var allCanBeResolved = true;
            var localUnresolvedDependencies = new List<Type>();
            var possibleDependencyResolvers = new List<Type>();

            for (var index = 0; index < _dependencies.Count; index++)
            {
                var dependent = _dependencies[index];
                possibleDependencyResolvers.Clear();

                for (var i = 0; i < index; i++)
                {
                    possibleDependencyResolvers.Add(_dependencies[i].GetResultingType());
                }

                localUnresolvedDependencies.Clear();
                if (DependencyUtils.DependenciesCanBeResolved(dependent, possibleDependencyResolvers,
                        localUnresolvedDependencies
                    )) continue;

                foreach (var localUnresolvedDependency in localUnresolvedDependencies)
                {
                    unresolvedDependencies.Add((dependent.GetResultingType(), localUnresolvedDependency));
                }

                allCanBeResolved = false;
            }

            return allCanBeResolved;
        }

        public bool TrySortTopologically(bool dryRun = false)
        {
            var graph = ListPool<List<int>>.Rent();

            for (var i = 0; i < _dependencies.Count; i++)
            {
                graph.Add(new List<int>());
            }

            for (var i = 0; i < _dependencies.Count; i++)
            {
                for (var j = 0; j < _dependencies.Count; j++)
                {
                    var dependent = _dependencies[i];
                    var dependency = _dependencies[j];
                    if (!DependencyUtils.DependsOn(dependent, dependency)) continue;

                    graph[j].Add(i);
                }
            }

            var result = ListPool<int>.Rent();
            SortTopologically(result, out var loop);

            if (loop)
            {
                ListPool<int>.Return(result);
                return false;
            }

            var sortedDependencies = ListPool<DependencyWithMetadata>.Rent();

            for (var resultIndex = result.Count - 1; resultIndex >= 0; resultIndex--)
            {
                var index = result[resultIndex];
                sortedDependencies.Add(_dependencies[index]);
            }

            if (!dryRun)
            {
                _dependencies.Clear();

                foreach (var dependency in sortedDependencies)
                {
                    _dependencies.Add(dependency);
                }
            }

            ListPool<DependencyWithMetadata>.Return(sortedDependencies);
            ListPool<int>.Return(result);
            return true;
        }

        internal void SortTopologically(ICollection<int> result, out bool loop)
        {
            var graph = ListPool<List<int>>.Rent();

            for (var i = 0; i < _dependencies.Count; i++)
            {
                graph.Add(ListPool<int>.Rent());
            }

            for (var i = 0; i < _dependencies.Count; i++)
            {
                for (var j = 0; j < _dependencies.Count; j++)
                {
                    var dependent = _dependencies[i];
                    var dependency = _dependencies[j];
                    if (!DependencyUtils.DependsOn(dependent, dependency)) continue;

                    graph[j].Add(i);
                }
            }

            TopologicalSorting.Sort(graph, _dependencies.Count, result, out loop);

            foreach (var graphList in graph)
            {
                ListPool<int>.Return(graphList);
            }

            ListPool<List<int>>.Return(graph);
        }

        public object GetOrCreateObject(int index)
        {
            ValidateIndex(index);
            return _dependencies[index].GetOrCreateObject(_resolutionFunction);
        }

        internal Type GetType(int index)
        {
            ValidateIndex(index);
            return _dependencies[index].GetResultingType();
        }

        internal HashSet<Type> GetTags(int index)
        {
            ValidateIndex(index);
            return _dependencies[index].Tags;
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= DependenciesCount) throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}