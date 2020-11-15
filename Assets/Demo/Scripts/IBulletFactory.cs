using UnityEngine;

namespace Demo.Scripts
{
	public interface IBulletFactory
	{
		Bullet Create(Vector3 position, Quaternion rotation);
		Bullet CreatePowerful(Vector3 position, Quaternion rotation);
	}
}