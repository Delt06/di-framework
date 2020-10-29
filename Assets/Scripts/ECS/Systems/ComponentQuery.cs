using System;
using System.Collections.Generic;
using ECS.Core;
using JetBrains.Annotations;

namespace ECS.Systems
{
	public sealed class ComponentQuery
	{
		public void AddType([NotNull] Type type)
		{
			if (IsFrozen) throw new InvalidOperationException("Cannot change a query if it is frozen.");
			if (type == null) throw new ArgumentNullException(nameof(type));
			_componentTypes.Add(type);
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