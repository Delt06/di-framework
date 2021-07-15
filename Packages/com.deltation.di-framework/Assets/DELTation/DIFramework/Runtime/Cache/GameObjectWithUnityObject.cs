using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Cache
{
    [Serializable]
    public struct GameObjectWithUnityObject
    {
        public GameObject GameObject;
        public Object Object;

        public GameObjectWithUnityObject(GameObject gameObject, Object @object)
        {
            GameObject = gameObject;
            Object = @object;
        }
    }
}