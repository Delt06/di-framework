using Components;
using ECS.Core;
using ECS.Dependencies;
using ECS.Systems;
using UnityEngine;

namespace Systems
{
	public sealed class CameraSystem : SystemBase<PlayerComponent> 
	{
		protected override void OnUpdate(IEntity entity, PlayerComponent player, float deltaTime, UpdateMode mode)
		{
			_camera.transform.position = player.transform.position + _offset;
		}

		protected override void OnStarted(IEntity entity)
		{
			base.OnStarted(entity);
			_offset = _camera.transform.position - entity.RequireComponent<PlayerComponent>().transform.position;
		}

		private Vector3 _offset;
		[Dependency] private readonly Camera _camera = default;
	}
}