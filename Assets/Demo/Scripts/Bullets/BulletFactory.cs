using UnityEngine;

namespace Demo.Scripts.Bullets
{
    public sealed class BulletFactory : IProjectileFactory<Bullet>
    {
        private readonly ILogger _logger;
        private readonly float _powerfulSpeedMultiplier;
        private readonly Bullet _prefab;
        private readonly Transform _transform;

        public BulletFactory(Transform transform, Bullet prefab, float powerfulSpeedMultiplier, ILogger logger)
        {
            _transform = transform;
            _prefab = prefab;
            _powerfulSpeedMultiplier = powerfulSpeedMultiplier;
            _logger = logger;
        }

        public Bullet Create(Vector3 position, Quaternion rotation)
        {
            _logger.Log($"Creating a bullet at {position}.");
            return Object.Instantiate(_prefab, position, rotation, _transform);
        }

        public Bullet CreatePowerful(Vector3 position, Quaternion rotation)
        {
            var bullet = Create(position, rotation);
            bullet.Speed *= _powerfulSpeedMultiplier;
            return bullet;
        }
    }
}