using UnityEngine;
using Component = UnityEngine.Component;

namespace DELTation.DIFramework.Containers
{
    /// <summary>
    /// Containers that registers all children.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Dependency Container/Children Dependency Container")]
    public sealed class ChildrenDependencyContainer : DependencyContainerBase
    {
        protected override void ComposeDependencies(ContainerBuilder builder)
        {
            foreach (var component in GetComponentsInChildren<Component>())
            {
                if (component == null) continue; // if a script is missing
                if (IsIgnored(component)) continue;
                builder.Register(component);
            }
        }

        private static bool IsIgnored(Component component) => component is Transform;
    }
}