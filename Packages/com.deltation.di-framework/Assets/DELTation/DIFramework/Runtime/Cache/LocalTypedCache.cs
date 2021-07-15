using System;

namespace DELTation.DIFramework.Cache
{
    internal sealed class LocalTypedCache : TypedCacheBase<GameObjectComponentType, GameObjectWithObject>
    {
        protected override bool AreCompatible(GameObjectComponentType key, GameObjectWithObject value) =>
            key.GameObject == value.GameObject;

        protected override Type GetType(in GameObjectComponentType key) => key.Type;

        protected override Type GetType(in GameObjectWithObject value) => value.Object.GetType();

        protected override GameObjectComponentType ToKey(in GameObjectWithObject value) =>
            new GameObjectComponentType(value.GameObject, GetType(value));
    }
}