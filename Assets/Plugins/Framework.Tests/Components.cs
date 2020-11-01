using Framework.Core;
using Framework.Dependencies;
using UnityEngine;

namespace Framework.Tests
{
	public class LocalDependencyComponent : ComponentBase
	{
		[Dependency(Source.Local)] public Rigidbody Rigidbody { get; private set; }
	}

	public class ParentDependencyComponent : ComponentBase
	{
		[Dependency(Source.Parents)] public Rigidbody Rigidbody { get; private set; }
	}

	public class ChildrenDependencyComponent : ComponentBase
	{
		[Dependency(Source.Children)] public Rigidbody Rigidbody { get; private set; }
	}

	public class EntityDependencyComponent : ComponentBase
	{
		[Dependency(Source.Entity)] public Rigidbody Rigidbody { get; private set; }
	}

	public class GlobalDependencyComponent : ComponentBase
	{
		[Dependency(Source.Global)] public Rigidbody Rigidbody { get; private set; }
	}
}