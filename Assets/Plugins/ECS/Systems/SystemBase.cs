using System.Collections.Generic;
using ECS.Core;
using ECS.Dependencies;
using ECS.Entities;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Systems
{
	public abstract class SystemBase : MonoBehaviour
	{
		public UpdateMode Mode = UpdateMode.Update;

		protected void LateUpdate()
		{
			foreach (var changedEntity in _changedEntities)
			{
				var isActive = EntityWorld.IsActive(changedEntity);
				if (isActive)
					_entityBuffer.Add(changedEntity);
				else
					_entityBuffer.Remove(changedEntity);
			}
			
			_changedEntities.Clear();
		}

		protected void Update()
		{
			UpdateAll(UpdateMode.Update, Time.deltaTime);
		}
		
		protected void FixedUpdate()
		{
			UpdateAll(UpdateMode.FixedUpdate, Time.fixedDeltaTime);
		}

		private void UpdateAll(UpdateMode mode, float deltaTime)
		{
			if ((Mode & mode) == 0) return;

			foreach (var entity in _entityBuffer)
			{
				OnUpdate(entity, deltaTime, mode);
			}
		}

		protected abstract void OnUpdate([NotNull] IEntity entity, float deltaTime, UpdateMode mode);
		
		protected void Start()
		{
			BuildQuery(_query);
			_query.Freeze();
			InvalidateBuffer();
			
			this.ResolveDependencies();
			OnStarted();
		}

		protected abstract void BuildQuery(ComponentQuery query);

		protected virtual void OnStarted() { }

		protected void Awake()
		{
			_onEntityChanged = entity =>
			{
				if (_query.IsSelected(entity))
					_changedEntities.Add(entity);
			};

			EntityWorld.Added += _onEntityChanged;
			EntityWorld.Removed += _onEntityChanged;
			
			OnAwake();
		}
		
		protected virtual void OnAwake() { }

		protected void OnDestroy()
		{
			EntityWorld.Added -= _onEntityChanged;
			EntityWorld.Removed -= _onEntityChanged;
			OnDestroyed();
		}
		
		protected virtual void OnDestroyed() { }

		private void InvalidateBuffer()
		{
			_entityBuffer.Clear();

			for (var index = 0; index < EntityWorld.EntitiesCount; index++)
			{
				var entity = EntityWorld.GetEntity(index);
				_entityBuffer.Add(entity);
			}
		}

		private EntityAction _onEntityChanged;
		private readonly HashSet<IEntity> _changedEntities = new HashSet<IEntity>();
		private readonly ComponentQuery _query = new ComponentQuery();
		private readonly HashSet<IEntity> _entityBuffer = new HashSet<IEntity>();
	}
}