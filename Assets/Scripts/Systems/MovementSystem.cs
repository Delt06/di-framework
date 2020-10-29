using Components;
using ECS.Components.BuiltIn;
using ECS.Core;
using ECS.Systems;

namespace Systems
{
	public sealed class MovementSystem : SystemBase<MovementComponent, RigidbodyComponent>
	{ 
		protected override void OnUpdate(IEntity entity, MovementComponent movementComponent, RigidbodyComponent rigidbodyComponent, float deltaTime,
			UpdateMode mode)
		{
			var body = rigidbodyComponent.Rigidbody;
			var velocity = movementComponent.Speed * movementComponent.Direction;
			velocity.y = body.velocity.y;
			body.velocity = velocity;
		}
	}
}