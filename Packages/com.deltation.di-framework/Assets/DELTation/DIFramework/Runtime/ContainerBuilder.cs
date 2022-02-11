using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DELTation.DIFramework.Pooling;
using DELTation.DIFramework.Resolution;
using DELTation.DIFramework.Sorting;
using JetBrains.Annotations;
using UnityEngine;
using static DELTation.DIFramework.Resolution.PocoInjection;

namespace DELTation.DIFramework
{
    /// <summary>
    ///     Allows to build a custom container.
    /// </summary>
    public sealed class ContainerBuilder
    {
        private readonly List<Dependency> _dependencies = new List<Dependency>();

        private readonly ResolutionFunction _resolutionFunction;
        internal readonly TagCollection<int> Tags = new TagCollection<int>();

        internal ContainerBuilder([NotNull] ResolutionFunction resolutionFunction) => _resolutionFunction =
            resolutionFunction ?? throw new ArgumentNullException(nameof(resolutionFunction));

        internal int DependenciesCount => _dependencies.Count;

        /// <summary>
        ///     Registers a new dependency of the given type.
        ///     An instance of that type will be automatically created.
        /// </summary>
        /// <typeparam name="T">Type of the dependency.</typeparam>
        /// <returns>The builder.</returns>
        public ContainerBuilder Register<[MeansImplicitUse] T>() where T : class
        {
            _dependencies.Add(new Dependency(typeof(T)));
            return this;
        }

        /// <summary>
        ///     Registers a new dependency that will be created using the given delegate.
        /// </summary>
        /// <param name="factoryMethod">A delegate that creates a dependency.</param>
        /// <returns>The builder.</returns>
        internal ContainerBuilder RegisterFromMethod([NotNull] Delegate factoryMethod)
        {
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            _dependencies.Add(new Dependency(factoryMethod));
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
                if (Dependency.DependenciesCanBeResolved(dependent, possibleDependencyResolvers,
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

        /// <summary>
        ///     Registers a new dependency from the given object.
        /// </summary>
        /// <param name="obj">Object to register as dependency.</param>
        /// <returns>The builder.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="obj" /> is null.</exception>
        public ContainerBuilder Register([NotNull] object obj)
        {
            var isNull = UnityUtils.IsNullOrUnityNull(obj);
            if (Application.isPlaying && isNull)
                throw new ArgumentNullException(nameof(obj));
            if (!isNull)
                _dependencies.Add(new Dependency(obj));
            return this;
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
                    if (!Dependency.DependsOn(dependent, dependency)) continue;

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

            var sortedDependencies = ListPool<Dependency>.Rent();

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

            ListPool<Dependency>.Return(sortedDependencies);
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
                    if (!Dependency.DependsOn(dependent, dependency)) continue;

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

            if (TryGetObject(index, out var obj))
                return obj;

            if (TryGetType(index, out var type))
            {
                var instance = CreateInstance(type);
                return instance;
            }

            if (TryGetFactoryMethodDelegate(index, out var factoryMethodDelegate))
            {
                var instance = factoryMethodDelegate.Instantiate(_resolutionFunction);
                return instance;
            }

            throw InvalidStateException();
        }

        private object CreateInstance(Type type) => PocoInjection.CreateInstance(type, _resolutionFunction);

        internal Type GetType(int index)
        {
            ValidateIndex(index);
            return _dependencies[index].GetResultingType();
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= DependenciesCount) throw new ArgumentOutOfRangeException(nameof(index));
        }

        private bool TryGetObject(int index, out object obj) => _dependencies[index].TryGetObject(out obj);

        private bool TryGetType(int index, out Type type) => _dependencies[index].TryGetType(out type);

        private bool TryGetFactoryMethodDelegate(int index, out FactoryMethodDelegate factoryMethodDelegate) =>
            _dependencies[index].TryGetFactoryMethodDelegate(out factoryMethodDelegate);

        private static InvalidOperationException InvalidStateException() =>
            new InvalidOperationException("Invalid state.");

        internal readonly struct Dependency : IEquatable<Dependency>
        {
            [CanBeNull] private readonly object _object;
            [CanBeNull] private readonly Type _type;
            private readonly FactoryMethodDelegate _factoryMethodDelegate;

            public Dependency([NotNull] object @object)
            {
                _object = @object ?? throw new ArgumentNullException(nameof(@object));
                _type = null;
                _factoryMethodDelegate = default;
            }

            public Dependency([NotNull] Type type)
            {
                _object = null;
                _type = type ?? throw new ArgumentNullException(nameof(type));
                _factoryMethodDelegate = default;
            }

            public Dependency(FactoryMethodDelegate factoryMethodDelegate)
            {
                _object = null;
                _type = null;
                _factoryMethodDelegate = factoryMethodDelegate;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Type GetResultingType()
            {
                if (TryGetObject(out var @object))
                    return @object.GetType();
                if (TryGetType(out var type))
                    return type;
                if (TryGetFactoryMethodDelegate(out var factoryMethodDelegate))
                    return factoryMethodDelegate.ReturnType;

                throw InvalidStateException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetObject(out object obj)
            {
                obj = _object;
                return obj != null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetType(out Type type)
            {
                type = _type;
                return type != null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetFactoryMethodDelegate(out FactoryMethodDelegate factoryMethodDelegate)
            {
                if (_factoryMethodDelegate.IsValid)
                {
                    factoryMethodDelegate = _factoryMethodDelegate;
                    return true;
                }

                factoryMethodDelegate = default;
                return false;
            }

            internal static bool DependenciesCanBeResolved(Dependency dependent, List<Type> possibleDependencyResolvers,
                List<Type> unresolvedDependencies)
            {
                if (dependent.TryGetObject(out _))
                    return true;

                if (dependent.TryGetType(out var type))
                    return TryGetInjectableConstructorParameters(type, out var parameters) &&
                           DependenciesCanBeResolved(parameters, possibleDependencyResolvers, unresolvedDependencies);

                if (dependent.TryGetFactoryMethodDelegate(out var factoryMethodDelegate))
                    return DependenciesCanBeResolved(factoryMethodDelegate.ParameterTypes, possibleDependencyResolvers,
                        unresolvedDependencies
                    );

                throw InvalidStateException();
            }

            private static bool DependenciesCanBeResolved(IEnumerable<ParameterInfo> dependencies,
                IReadOnlyCollection<Type> possibleDependencyResolvers, List<Type> unresolvedDependencies)
            {
                bool CanBeResolved(ParameterInfo dependency) =>
                    possibleDependencyResolvers.Any(possibleDependency =>
                        dependency.ParameterType.IsAssignableFrom(possibleDependency)
                    );

                var canResolveAll = true;

                foreach (var dependency in dependencies)
                {
                    if (CanBeResolved(dependency)) continue;

                    canResolveAll = false;
                    unresolvedDependencies.Add(dependency.ParameterType);
                }

                return canResolveAll;
            }

            public static bool DependsOn(Dependency dependent, Dependency dependency)
            {
                if (dependent.TryGetObject(out _))
                    return false;

                var dependencyType = dependency.GetResultingType();
                if (dependent.TryGetType(out var type))
                    return TryGetInjectableConstructorParameters(type, out var parameters) &&
                           DependsOn(parameters, dependencyType);

                if (dependent.TryGetFactoryMethodDelegate(out var factoryMethodDelegate))
                    return DependsOn(factoryMethodDelegate.ParameterTypes, dependencyType);

                throw InvalidStateException();
            }

            private static bool DependsOn([NotNull] IReadOnlyList<ParameterInfo> dependents, [NotNull] Type dependency)
            {
                if (dependents == null) throw new ArgumentNullException(nameof(dependents));
                if (dependency == null) throw new ArgumentNullException(nameof(dependency));

                foreach (var dependent in dependents)
                {
                    if (dependent.ParameterType.IsAssignableFrom(dependency))
                        return true;
                }

                return false;
            }

            public bool Equals(Dependency other) => Equals(_object, other._object) &&
                                                    _type == other._type &&
                                                    _factoryMethodDelegate.Equals(other._factoryMethodDelegate);

            public override bool Equals(object obj) => obj is Dependency other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _object != null ? _object.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ (_type != null ? _type.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ _factoryMethodDelegate.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}