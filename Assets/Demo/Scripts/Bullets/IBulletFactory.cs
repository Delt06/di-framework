using UnityEngine;

namespace Demo.Scripts.Bullets
{
	public interface IBulletFactory
	{
		Bullet Create(Vector3 position, Quaternion rotation);
		Bullet CreatePowerful(Vector3 position, Quaternion rotation);
	}
}