using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Resolution;
using DELTation.DIFramework.Sorting;
using JetBrains.Annotations;

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

        public void SortTopologically()
        {
            var graph = new List<int>[_dependencies.Count];

            for (var i = 0; i < _dependencies.Count; i++)
            {
                graph[i] = new List<int>();
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

            var result = new LinkedList<int>();
            TopologicalSorting.Sort(graph, _dependencies.Count, result, out var loop);
            if (loop)
                throw new InvalidOperationException("Dependencies contain a loop.");

            _dependencies = result.Select(i => _dependencies[i]).ToList();
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

        private object CreateInstance(Type type)
        {
            if (!TryGetInjectableConstructorParameters(type, out var parameters))
                throw new ArgumentException($"Type {type} does not have an injectable constructor.");

            var arguments = new object[parameters.Length];

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameterType = parameters[index].ParameterType;
                if (_container.TryResolve(parameterType, out var dependency))
                    arguments[index] = dependency;
                else
                    throw new DependencyNotRegisteredException(parameterType);
            }

            return Activator.CreateInstance(type, arguments);
        }

        private static bool TryGetInjectableConstructorParameters(Type type, out ParameterInfo[] parameters)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ParameterInfo[] foundParameters = null;

            foreach (var constructor in constructors)
            {
                parameters = constructor.GetParameters();
                if (!parameters.AreInjectable()) continue;

                if (foundParameters == null)
                    foundParameters = parameters;
                else
                    return false;
            }

            parameters = foundParameters;
            return foundParameters != null;
        }

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

        internal ContainerBuilder([NotNull] IDependencyContainer container) =>
            _container = container ?? throw new ArgumentNullException(nameof(container));

        private readonly IDependencyContainer _container;
        private List<Dependency> _dependencies = new List<Dependency>();

        private readonly struct Dependency
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
        }
    }
}