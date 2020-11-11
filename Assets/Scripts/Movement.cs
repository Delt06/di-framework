using UnityEngine;

public sealed class Movement : MonoBehaviour
{
	[SerializeField, Min(0f)] private float _speed = 1f;

	private void FixedUpdate()
	{
		var velocity = _rigidbody.velocity;
		velocity.x = Input.GetAxisRaw("Horizontal") * _speed;
		velocity.z = Input.GetAxisRaw("Vertical") * _speed;
		_rigidbody.velocity = velocity;
	}

	public void Construct(Rigidbody rigidbody)
	{
		_rigidbody = rigidbody;
	}

	private Rigidbody _rigidbody;
}