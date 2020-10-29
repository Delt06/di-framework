using ECS.Components;
using UnityEngine;

namespace Components
{
	public sealed class MovementComponent : ComponentBase
	{
		public Vector3 Direction = Vector3.zero;
		public float Speed = 1f;
	}
}