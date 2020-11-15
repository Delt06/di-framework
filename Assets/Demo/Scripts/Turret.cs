using UnityEngine;

namespace Demo.Scripts
{
	public sealed class Turret : MonoBehaviour
	{
		[SerializeField, Min(0f)] private float _shootingPeriod = 1f;
		[SerializeField, Min(1)] private int _powerfulEveryShot = 5;

		private void Start()
		{
			var time = Random.Range(_shootingPeriod, _shootingPeriod * 2f);
			InvokeRepeating(nameof(Shoot), time, _shootingPeriod);
		}

		private void Shoot()
		{
			_shots++;

			if (_shots % _powerfulEveryShot == 0)
				_bulletFactory.CreatePowerful(_shootFrom.Position, _shootFrom.Rotation);
			else
				_bulletFactory.Create(_shootFrom.Position, _shootFrom.Rotation);
		}

		public void Construct(IBulletFactory bulletFactory, IShootFrom shootFrom)
		{
			_bulletFactory = bulletFactory;
			_shootFrom = shootFrom;
		}

		private int _shots;
		private IBulletFactory _bulletFactory;
		private IShootFrom _shootFrom;
	}
}