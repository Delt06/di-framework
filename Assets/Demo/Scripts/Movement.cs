using UnityEngine;

namespace Demo.Scripts
{
	public sealed class Movement : MonoBehaviour, IMovement
	{
		[SerializeField, Min(0f)] private float _speed = 1f;

		public float Vertical { get; set; }
		public float Horizontal { get; set; }

		private void FixedUpdate()
		{
			var direction = new Vector3(Horizontal, 0f, Vertical);
			direction.Normalize();
			var velocity = direction * _speed;
			velocity.y = _rigidbody.velocity.y;
			_rigidbody.velocity = velocity;
		}

		public void Construct(Rigidbody rigidbody)
		{
			_rigidbody = rigidbody;
		}

		private Rigidbody _rigidbody;
	}
}