using System;
using System.Collections.Generic;
using ECS.Core;
using JetBrains.Annotations;

namespace ECS.Systems
{
	public sealed class ComponentQuery
	{
		public ComponentQuery AddType([NotNull] Type type)
		{
			if (IsFrozen) throw new InvalidOperationException("Cannot change a query if it is frozen.");
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (!typeof(IComponent).IsAssignableFrom(type)) throw new ArgumentException($"Type has to derive from {nameof(IComponent)}.");
			_componentTypes.Add(type);
			return this;
		}

		public ComponentQuery AddType<T>() where T : IComponent
		{
			AddType(typeof(T));
			return this;
		}

		public void Freeze() => IsFrozen = true;
		
		public bool IsFrozen { get; private set; }

		public bool IsSelected([NotNull] IEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			foreach (var componentType in _componentTypes)
			{
				if (!entity.TryFindComponent(componentType, out _))
					return false;
			}

			return true;
		}

		private readonly HashSet<Type> _componentTypes = new HashSet<Type>();
	}
}