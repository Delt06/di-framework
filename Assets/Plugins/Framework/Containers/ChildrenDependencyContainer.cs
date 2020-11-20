using UnityEngine;
using Component = UnityEngine.Component;

namespace Framework.Containers
{
	[DisallowMultipleComponent, AddComponentMenu("Dependency Container/Children Dependency Container")]
	public sealed class ChildrenDependencyContainer : DependencyContainerBase
	{
		protected override void ComposeDependencies(ContainerBuilder builder)
		{
			foreach (var component in GetComponentsInChildren<Component>())
			{
				if (IsIgnored(component)) continue;
				builder.Register(component);
			}
		}

		private static bool IsIgnored(Component component) => component is Transform;
	}
}