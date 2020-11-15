using UnityEngine;

namespace Demo.Scripts
{
	public class Player : MonoBehaviour, IShootingTarget
	{
		public Vector3 Position => transform.position;
	}
}