using UnityEngine;
using Component = UnityEngine.Component;

namespace Framework.Dependencies.Containers
{
	[AddComponentMenu("Dependency Container/Children Dependency Container")]
	public sealed class ChildrenDependencyContainer : DependencyContainerBase
	{
		protected override void ComposeDependencies()
		{
			foreach (var component in GetComponentsInChildren<Component>())
			{
				if (component is IDependencyContainer) continue;
				
				Register(component);
			}
		}
	}
}