using ECS.Components;
using ECS.Dependencies;
using UnityEngine;

namespace Demo
{
	public sealed class LightComponent : ComponentBase
	{
		private void Start()
		{
			_light.color = Color.red;
			_meshRenderer.transform.localScale *= 2f;
		}

		[Dependency] private readonly Light _light = default;
		[Dependency(Source.Entity)] private readonly MeshRenderer _meshRenderer = default;
	}
}