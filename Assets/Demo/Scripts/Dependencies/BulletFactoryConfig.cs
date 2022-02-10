using Demo.Scripts.Bullets;
using UnityEngine;

namespace Demo.Scripts.Dependencies
{
    [CreateAssetMenu]
    public class BulletFactoryConfig : ScriptableObject
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private float _powerfulBulletSpeedMultiplier;

        public Bullet BulletPrefab => _bulletPrefab;

        public float PowerfulBulletSpeedMultiplier => _powerfulBulletSpeedMultiplier;
    }
}