using Demo.Scripts.Shooting;
using UnityEngine;

namespace Demo.Scripts.Turrets
{
	public class TurretHead : MonoBehaviour, IShootFrom
	{
		[SerializeField] private Vector3 _offset = Vector3.zero;

		public Vector3 Position => transform.position + Rotation * _offset;
		public Quaternion Rotation => transform.rotation;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(Position, Rotation * Vector3.forward);
		}
	}
}