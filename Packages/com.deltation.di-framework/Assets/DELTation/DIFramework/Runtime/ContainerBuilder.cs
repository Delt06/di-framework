using System;
using System.Collections.Generic;
using DELTation.DIFramework.Pooling;
using DELTation.DIFramework.Resolution;
using DELTation.DIFramework.Sorting;
using JetBrains.Annotations;
using static DELTation.DIFramework.Resolution.PocoInjection;

namespace DELTation.DIFramework
{
    /// <summary>
    /// Allows to build a custom container.
    /// </summary>
    public sealed class ContainerBuilder
    {
        /// <summary>
        /// Registers a new dependency of the given type.
        /// An instance of that type will be automatically created.
        /// </summary>
        /// <typeparam name="T">Type of the dependency.</typeparam>
        /// <returns>The builder.</returns>
        public ContainerBuilder Register<T>() where T : class
        {
            _dependencies.Add(new Dependency(typeof(T)));
            return this;
        }

        /// <summary>
        /// Registers a new dependency from the given object.
        /// </summary>
        /// <param name="obj">Object to register as dependency.</param>
        /// <returns>The builder.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="obj"/> is null.</exception>
        public ContainerBuilder Register([NotNull] object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
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
                    var dependency1 = _dependencies[i];
                    var dependency2 = _dependencies[j];

                    var type1 = dependency1.GetTypeOrObjectType();
                    var type2 = dependency2.GetTypeOrObjectType();
                    if (!Dependency.DependsOn(type2, type1)) continue;

                    graph[i].Add(j);
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

        private void SortTopologically(ICollection<int> result, out bool loop)
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
                    var dependency1 = _dependencies[i];
                    var dependency2 = _dependencies[j];

                    var type1 = dependency1.GetTypeOrObjectType();
                    var type2 = dependency2.GetTypeOrObjectType();
                    if (!Dependency.DependsOn(type2, type1)) continue;

                    graph[i].Add(j);
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

            throw InvalidStateException();
        }

        private object CreateInstance(Type type) => PocoInjection.CreateInstance(type, _resolutionFunction);

        internal Type GetType(int index)
        {
            ValidateIndex(index);

            if (TryGetObject(index, out var obj))
                return obj.GetType();

            if (TryGetType(index, out var type))
                return type;

            throw InvalidStateException();
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= DependenciesCount) throw new ArgumentOutOfRangeException(nameof(index));
        }

        internal int DependenciesCount => _dependencies.Count;

        private bool TryGetObject(int index, out object obj) => _dependencies[index].TryGetObject(out obj);

        private bool TryGetType(int index, out Type type) => _dependencies[index].TryGetType(out type);

        private static InvalidOperationException InvalidStateException() =>
            new InvalidOperationException("Invalid state: either object or type should be not null.");

        internal ContainerBuilder([NotNull] ResolutionFunction resolutionFunction) => _resolutionFunction =
            resolutionFunction ?? throw new ArgumentNullException(nameof(resolutionFunction));

        private readonly ResolutionFunction _resolutionFunction;
        private readonly List<Dependency> _dependencies = new List<Dependency>();

        private readonly struct Dependency : IEquatable<Dependency>
        {
            [CanBeNull] private readonly object _object;
            [CanBeNull] private readonly Type _type;

            public Dependency([NotNull] object @object)
            {
                _object = @object ?? throw new ArgumentNullException(nameof(@object));
                _type = null;
            }

            public Dependency([NotNull] Type type)
            {
                _object = null;
                _type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public Type GetTypeOrObjectType() => _object != null ? _object.GetType() : _type;

            public bool TryGetObject(out object obj)
            {
                obj = _object;
                return obj != null;
            }

            public bool TryGetType(out Type type)
            {
                type = _type;
                return type != null;
            }

            public static bool DependsOn([NotNull] Type type1, [NotNull] Type type2)
            {
                if (type1 == null) throw new ArgumentNullException(nameof(type1));
                if (type2 == null) throw new ArgumentNullException(nameof(type2));
                if (!TryGetInjectableConstructorParameters(type1, out var parameters)) return false;

                foreach (var parameter in parameters)
                {
                    var parameterType = parameter.ParameterType;
                    if (parameterType.IsAssignableFrom(type2))
                        return true;
                }

                return false;
            }

            public bool Equals(Dependency other) => Equals(_object, other._object) && _type == other._type;

            public override bool Equals(object obj) => obj is Dependency other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_object != null ? _object.GetHashCode() : 0) * 397) ^
                           (_type != null ? _type.GetHashCode() : 0);
                }
            }
        }
    }
}