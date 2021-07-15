using System;
using UnityEngine;

namespace DELTation.DIFramework.Cache
{
    internal readonly struct GameObjectComponentType : IEquatable<GameObjectComponentType>
    {
        public readonly GameObject GameObject;
        public readonly Type Type;

        public GameObjectComponentType(GameObject gameObject, Type type)
        {
            GameObject = gameObject;
            Type = type;
        }

        public bool Equals(GameObjectComponentType other) => GameObject == other.GameObject && Type == other.Type;

        public override bool Equals(object obj) => obj is GameObjectComponentType other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((GameObject != null ? GameObject.GetHashCode() : 0) * 397) ^
                       (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}