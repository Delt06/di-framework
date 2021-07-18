using UnityEngine;

namespace Demo.Scripts.Bullets
{
    public sealed class BulletFactory : MonoBehaviour, IProjectileFactory<Bullet>
    {
        [SerializeField] private Bullet _prefab = default;
        [SerializeField, Min(0f)] private float _powerfulSpeedMultiplier = 2f;

        public Bullet Create(Vector3 position, Quaternion rotation) =>
            Instantiate(_prefab, position, rotation, transform);

        public Bullet CreatePowerful(Vector3 position, Quaternion rotation)
        {
            var bullet = Create(position, rotation);
            bullet.Speed *= _powerfulSpeedMultiplier;
            return bullet;
        }
    }
}