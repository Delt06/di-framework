using Demo.Scripts.Damage;
using UnityEngine;

namespace Demo.Scripts.Bullets
{
	public class BulletHitTrigger : MonoBehaviour
	{
		public void Construct(Bullet bullet)
		{
			_bullet = bullet;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.TryGetComponent(out IHittable hittable)) return;
			hittable.Hit();
			_bullet.Destroy();
		}

		private Bullet _bullet;
	}
}