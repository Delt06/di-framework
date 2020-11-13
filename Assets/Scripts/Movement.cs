using UnityEngine;

public sealed class Movement : MonoBehaviour
{
	[SerializeField, Min(0f)] private float _speed = 1f;

	private void FixedUpdate()
	{
		var velocity = _rigidbody.velocity;
		var direction = new Vector2(_inputSource.HorizontalAxis, _inputSource.VerticalAxis);
		direction.Normalize();
		velocity.x = direction.x * _speed;
		velocity.z = direction.y * _speed;
		_rigidbody.velocity = velocity;
	}

	public void Construct(Rigidbody rigidbody, IInputSource inputSource, ISpeedProvider speedProvider)
	{
		_rigidbody = rigidbody;
		_inputSource = inputSource;
		_speedProvider = speedProvider;
	}

	private IInputSource _inputSource;
	private Rigidbody _rigidbody;
	private ISpeedProvider _speedProvider;
}