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

		internal int DependenciesCount => _dependencies.Count;

		internal bool TryGetObject(int index, out object obj)
		{
			if (index < 0 || index >= DependenciesCount) throw new ArgumentOutOfRangeException(nameof(index));
			obj = _dependencies[index].obj;
			return obj != null;
		}

		internal bool TryGetType(int index, out Type type)
		{
			if (index < 0 || index >= DependenciesCount) throw new ArgumentOutOfRangeException(nameof(index));
			type = _dependencies[index].type;
			return type != null;
		}

		private readonly List<(object obj, Type type)> _dependencies = new List<(object obj, Type type)>();
	}
}