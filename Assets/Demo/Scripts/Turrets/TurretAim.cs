using Demo.Scripts.Shooting;
using UnityEngine;

namespace Demo.Scripts.Turrets
{
	public sealed class TurretAim : MonoBehaviour
	{
		[SerializeField] private float _angularSpeed = 90f;

		private void Update()
		{
			if (!_target.IsActive) return;
			var deltaRotation = _angularSpeed * Time.deltaTime;
			_transform.rotation = RotateTowards(TargetRotation, deltaRotation);
		}

		private Quaternion TargetRotation
		{
			get
			{
				var offset = _target.Position - _transform.position;
				return Quaternion.LookRotation(offset, Vector3.up);
			}
		}

		private Quaternion RotateTowards(Quaternion targetRotation, float deltaRotation) =>
			Quaternion.RotateTowards(_transform.rotation, targetRotation, deltaRotation);

		private void Awake()
		{
			_transform = transform;
		}

		public void Construct(IShootingTarget target)
		{
			_target = target;
		}

		private Transform _transform;
		private IShootingTarget _target;
	}
}