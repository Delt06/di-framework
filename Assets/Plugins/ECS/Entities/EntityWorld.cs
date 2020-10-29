using System;
using System.Collections.Generic;
using ECS.Core;
using JetBrains.Annotations;

namespace ECS.Entities
{
	public static class EntityWorld
	{
		public static int EntitiesCount => Entities.Count;

		public static IEntity GetEntity(int index)
		{
			if (index < 0 || index >= EntitiesCount) throw new ArgumentOutOfRangeException(nameof(index));
			return Entities[index];
		}

		public static bool IsActive([NotNull] IEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			return EntitySet.Contains(entity);
		}
		
		public static void Add([NotNull] IEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			if (EntitySet.Contains(entity)) return;
			
			Entities.Add(entity);
			EntitySet.Add(entity);
			entity.EnsureInitialized();
			
			Added?.Invoke(entity);
		}

		public static event EntityAction Added;

		public static void Remove([NotNull] IEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			if (!EntitySet.Contains(entity)) return;
			
			Entities.Remove(entity);
			EntitySet.Remove(entity);
			
			Removed?.Invoke(entity);
		}
		
		public static event EntityAction Removed;
		
		private static readonly HashSet<IEntity> EntitySet = new HashSet<IEntity>();
		private static readonly List<IEntity> Entities = new List<IEntity>();
	}
}