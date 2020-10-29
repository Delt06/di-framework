using Components;
using ECS.Core;
using ECS.Systems;
using UnityEngine;

namespace Systems
{
	public sealed class InputSystem : SystemBase<MovementComponent, PlayerComponent>
	{
		[SerializeField, Min(0f)] private float _speed = 1f; 
		
		protected override void OnUpdate(IEntity entity, MovementComponent movement, PlayerComponent component2, float deltaTime,
			UpdateMode mode)
		{
			var x = Input.GetAxisRaw("Horizontal");
			var y = Input.GetAxisRaw("Vertical");
			var direction = new Vector3(x, 0f, y).normalized;
			movement.Direction = direction;
			movement.Speed = _speed;
		}
	}
}