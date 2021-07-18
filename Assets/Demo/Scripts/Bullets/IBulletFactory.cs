using UnityEngine;

namespace Demo.Scripts.Bullets
{
    public interface IProjectileFactory<out TProjectile>
    {
        TProjectile Create(Vector3 position, Quaternion rotation);
        TProjectile CreatePowerful(Vector3 position, Quaternion rotation);
    }
}