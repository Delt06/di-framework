using UnityEngine;

namespace Demo.Scripts
{
	public sealed class MovementControls : MonoBehaviour
	{
		private void Update()
		{
			_movement.Horizontal = Input.GetAxisRaw("Horizontal");
			_movement.Vertical = Input.GetAxisRaw("Vertical");
		}

		public void Construct(IMovement movement)
		{
			_movement = movement;
		}

		private IMovement _movement;
	}
}