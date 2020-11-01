using Framework.Core;
using Framework.Dependencies;
using UnityEngine;

public sealed class Movement : ComponentBase
{
	[SerializeField, Min(0f)] private float _speed = 1f;

	private void FixedUpdate()
	{
		var velocity = _rigidbody.velocity;
		velocity.x = Input.GetAxisRaw("Horizontal") * _speed;
		velocity.z = Input.GetAxisRaw("Vertical") * _speed;
		_rigidbody.velocity = velocity;
	}

	[Dependency(Source.Entity)] private readonly Rigidbody _rigidbody = default;
}