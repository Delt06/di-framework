using System;

namespace Demo.Scripts.Damage
{
    public interface IDestroyable
    {
        event EventHandler OnPreDestroyed;
    }
}