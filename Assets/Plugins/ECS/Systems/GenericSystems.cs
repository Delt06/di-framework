using ECS.Core;
using JetBrains.Annotations;

namespace ECS.Systems
{
	public abstract class SystemBase<T> : SystemBase where T : class, IComponent
	{
		protected sealed override void OnUpdate(IEntity entity, float deltaTime, UpdateMode mode)
		{
			OnUpdate(entity, entity.RequireComponent<T>(), deltaTime, mode);
		}

		protected abstract void OnUpdate([NotNull] IEntity entity, [NotNull] T component, float deltaTime,
			UpdateMode mode);

		protected sealed override void BuildQuery(ComponentQuery query)
		{
			query.AddType<T>();
		}
	}
	
	public abstract class SystemBase<T1, T2> : SystemBase where T1 : class, IComponent
		where T2 : class, IComponent
	{
		protected sealed override void OnUpdate(IEntity entity, float deltaTime, UpdateMode mode)
		{
			OnUpdate(entity, entity.RequireComponent<T1>(), entity.RequireComponent<T2>(), deltaTime, mode);
		}

		protected abstract void OnUpdate([NotNull] IEntity entity, [NotNull] T1 component1, [NotNull] T2 component2, float deltaTime,
			UpdateMode mode);

		protected sealed override void BuildQuery(ComponentQuery query)
		{
			query.AddType<T1>()
				.AddType<T2>();
		}
	}
	
	public abstract class SystemBase<T1, T2, T3, T4> : SystemBase where T1 : class, IComponent
		where T2 : class, IComponent
		where T3 : class, IComponent
		where T4 : class, IComponent
	{
		protected sealed override void OnUpdate(IEntity entity, float deltaTime, UpdateMode mode)
		{
			OnUpdate(entity, entity.RequireComponent<T1>(), entity.RequireComponent<T2>(), entity.RequireComponent<T3>(), entity.RequireComponent<T4>(), deltaTime, mode);
		}

		protected abstract void OnUpdate([NotNull] IEntity entity, [NotNull] T1 component1, 
			[NotNull] T2 component2, [NotNull] T3 component3, [NotNull] T4 component4, 
			float deltaTime, UpdateMode mode);

		protected sealed override void BuildQuery(ComponentQuery query)
		{
			query.AddType<T1>()
				.AddType<T2>()
				.AddType<T3>()
				.AddType<T4>();
		}
	}
}