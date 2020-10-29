using System;

namespace ECS.Systems
{
	[Flags]
	public enum UpdateMode
	{
		Update = 1 << 1, 
		FixedUpdate = 1 << 2,
		LateUpdate = 1 << 3
	}
}