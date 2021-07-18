using Demo.Scripts.Bullets;
using Demo.Scripts.Shooting;
using UnityEngine;

namespace Demo.Scripts.Turrets
{
    public sealed class Turret : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _shootingPeriod = 1f;
        [SerializeField, Range(0f, 1f)] private float _powerfulShotProbability = 0.1f;

        public void Construct(IProjectileFactory<Bullet> bulletFactory, IShootFrom shootFrom, IShootingTarget target)
        {
            _bulletFactory = bulletFactory;
            _shootFrom = shootFrom;
            _target = target;
        }

        private void Start()
        {
            var time = Random.Range(_shootingPeriod, _shootingPeriod * 2f);
            InvokeRepeating(nameof(Shoot), time, _shootingPeriod);
        }

        private void Shoot()
        {
            if (!_target.IsActive) return;

            if (Random.value <= _powerfulShotProbability)
                _bulletFactory.CreatePowerful(_shootFrom.Position, _shootFrom.Rotation);
            else
                _bulletFactory.Create(_shootFrom.Position, _shootFrom.Rotation);
        }

        private IProjectileFactory<Bullet> _bulletFactory;
        private IShootingTarget _target;
        private IShootFrom _shootFrom;
    }
}