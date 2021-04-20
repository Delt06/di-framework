using UnityEngine;

namespace Demo.Scripts.Shooting
{
    public interface IShootingTarget
    {
        bool IsActive { get; }
        Vector3 Position { get; }
    }
}