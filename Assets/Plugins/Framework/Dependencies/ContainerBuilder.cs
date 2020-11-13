using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Framework.Dependencies
{
	public sealed class ContainerBuilder
	{
		public ContainerBuilder Register<T>() where T : class, new()
		{
			_dependencies.Add((null, typeof(T)));
			return this;
		}

		public ContainerBuilder Register([NotNull] object obj)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			_dependencies.Add((obj, null));
			return this;
		}

		internal object GetOrCreateObject(int index)
		{
			ValidateIndex(index);

			if (TryGetObject(index, out var obj))
				return obj;

			if (TryGetType(index, out var type))
				return Activator.CreateInstance(type);

			throw InvalidStateException();
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

		private bool TryGetObject(int index, out object obj)
		{
			obj = _dependencies[index].obj;
			return obj != null;
		}

		private bool TryGetType(int index, out Type type)
		{
			type = _dependencies[index].type;
			return type != null;
		}

		private static InvalidOperationException InvalidStateException() =>
			new InvalidOperationException("Invalid state: either object or type should be not null.");

		private readonly List<(object obj, Type type)> _dependencies = new List<(object obj, Type type)>();
	}
}