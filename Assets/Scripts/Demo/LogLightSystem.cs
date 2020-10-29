using ECS.Core;
using ECS.Dependencies;
using ECS.Systems;
using UnityEngine;

namespace Demo
{
	public class LogLightSystem : SystemBase
	{
		[SerializeField] private float _angularSpeed = 90f;
		
		protected override void OnUpdate(IEntity entity, float deltaTime, UpdateMode mode)
		{
			Debug.Log(entity + " at frame " + _frames);
			_frames++;
			_camera.transform.Rotate(Vector3.up, _angularSpeed * deltaTime);
		}

		protected override void BuildQuery(ComponentQuery query)
		{
			query.AddType(typeof(LightComponent));
		}

		private int _frames;
		[Dependency] private readonly Camera _camera = default;
	}
}