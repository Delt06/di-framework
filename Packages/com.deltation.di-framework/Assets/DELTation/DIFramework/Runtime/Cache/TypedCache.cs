using System;

namespace DELTation.DIFramework.Cache
{
    internal sealed class TypedCache : TypedCacheBase<Type, object>
    {
        public bool TryGet<T>(out T obj) where T : class
        {
            if (TryGet(typeof(T), out var foundObject))
            {
                obj = (T) foundObject;
                return true;
            }

            obj = default;
            return false;
        }

        protected override bool AreCompatible(Type key, object value) => true;

        protected override Type GetType(in Type key) => key;

        protected override Type GetType(in object value) => value.GetType();

        protected override Type ToKey(in object value) => value.GetType();
    }
}