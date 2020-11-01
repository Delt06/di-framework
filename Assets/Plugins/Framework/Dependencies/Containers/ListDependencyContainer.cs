using UnityEngine;

namespace Framework.Dependencies.Containers
{
	[AddComponentMenu("Dependency Container/List Dependency Container")]
	public sealed class ListDependencyContainer : DependencyContainerBase
	{
		[SerializeField] private Object[] _dependencies = default;
		
		protected override void ComposeDependencies()
		{
			for (var index = 0; index < _dependencies.Length; index++)
			{
				if (_dependencies[index] == null)
				{
					Debug.LogError($"Dependency at index {index} is null.", this);
					continue;
				}

				var dependency = _dependencies[index];

				switch (dependency)
				{
					case GameObject go:
						foreach (var c in go.GetComponents<Component>())
						{
							Register(c);
						}
						break;
					
					case Component component:
						foreach (var c in component.GetComponents<Component>())
						{
							Register(c);
						}
						break;
					
					default:
						Register(dependency);
						break;
				}
			}
		}
	}
}