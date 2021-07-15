using System;
using UnityEngine;

namespace DELTation.DIFramework.Cache
{
    public readonly struct GameObjectWithObject : IEquatable<GameObjectWithObject>
    {
        public readonly GameObject GameObject;
        public readonly object Object;

        public GameObjectWithObject(GameObject gameObject, object @object)
        {
            GameObject = gameObject;
            Object = @object;
        }

        public bool Equals(GameObjectWithObject other) =>
            GameObject == other.GameObject && Equals(Object, other.Object);

        public override bool Equals(object obj) => obj is GameObjectWithObject other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((GameObject != null ? GameObject.GetHashCode() : 0) * 397) ^
                       (Object != null ? Object.GetHashCode() : 0);
            }
        }
    }
}