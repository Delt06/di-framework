using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using static Framework.Dependencies.DependencyExceptionFactory;

namespace Framework.Dependencies
{
	public sealed class ContainerBuilder
	{
		public ContainerBuilder Register<T>() where T : class
		{
			_dependencies.Add(new Dependency(typeof(T)));
			return this;
		}

		public ContainerBuilder Register([NotNull] object obj)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			_dependencies.Add(new Dependency(obj));
			return this;
		}

		internal void SortTopologically() => _dependencies.Sort();

		internal object GetOrCreateObject(int index)
		{
			ValidateIndex(index);

			if (TryGetObject(index, out var obj))
				return obj;

			if (TryGetType(index, out var type))
				return CreateInstance(type);

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
					throw NotRegistered(type);
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
		private readonly List<Dependency> _dependencies = new List<Dependency>();

		private readonly struct Dependency : IEquatable<Dependency>, IComparable<Dependency>
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

			public override bool Equals(object obj) => obj is Dependency other && Equals(other);

			public bool Equals(Dependency other) => Equals(_object, other._object) && _type == other._type;

			public override int GetHashCode()
			{
				unchecked
				{
					return ((_object != null ? _object.GetHashCode() : 0) * 397) ^
					       (_type != null ? _type.GetHashCode() : 0);
				}
			}

			public int CompareTo(Dependency other)
			{
				if (TryGetObject(out _) && !other.TryGetObject(out _))
					return -1;

				if (!TryGetObject(out _) && other.TryGetObject(out _))
					return 1;

				if (TryGetObject(out _) && other.TryGetObject(out _))
					return 0;

				if (!TryGetType(out var type1)) return 0;
				if (!TryGetType(out var type2)) return 0;

				if (DependsOn(type1, type2)) return -1;
				if (DependsOn(type2, type1)) return 1;

				return 0;
			}

			private static bool DependsOn(Type type1, Type type2)
			{
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