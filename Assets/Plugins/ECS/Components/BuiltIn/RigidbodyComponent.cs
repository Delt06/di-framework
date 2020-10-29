
using ECS.Dependencies;
using UnityEngine;

namespace ECS.Components.BuiltIn
{
	public sealed class RigidbodyComponent : ComponentBase
	{
		[Dependency(Source.Entity)]
		public Rigidbody Rigidbody { get; private set; } = default;
	}
}