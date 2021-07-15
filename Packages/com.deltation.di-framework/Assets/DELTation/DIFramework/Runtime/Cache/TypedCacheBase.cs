using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Cache
{
    internal abstract class TypedCacheBase<TKey, TValue>
    {
        internal void InitializeFrom([NotNull] IReadOnlyList<TValue> objects)
        {
            Clear();

            for (var index = 0; index < objects.Count; index++)
            {
                _allObjects.Add(objects[index]);
            }
        }

        internal void AddAllObjectsTo([NotNull] ICollection<TValue> targetList)
        {
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));

            foreach (var @object in _allObjects)
            {
                targetList.Add(@object);
            }
        }

        internal bool TryGet([NotNull] in TKey key, out TValue obj)
        {
            if (_objects.TryGetValue(key, out obj))
                return true;

            if (TryFindInSubclasses(key, out obj))
                return true;

            obj = default;
            return false;
        }

        private bool TryFindInSubclasses([NotNull] in TKey key, out TValue obj)
        {
            var type = GetType(key);

            foreach (var existingObject in _allObjects)
            {
                if (!AreCompatible(key, existingObject)) continue;

                var typeOfExistingObject = GetType(existingObject);
                if (!type.IsAssignableFrom(typeOfExistingObject)) continue;

                obj = existingObject;
                _objects[key] = existingObject;
                return true;
            }

            obj = default;
            return false;
        }

        protected abstract bool AreCompatible(TKey key, TValue value);

        protected abstract Type GetType(in TKey key);
        protected abstract Type GetType(in TValue value);

        public bool TryRegister([NotNull] TValue obj, out TValue existingObject)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var key = ToKey(obj);
            if (_objects.TryGetValue(key, out existingObject))
                return false;

            _allObjects.Add(obj);
            _objects[key] = obj;
            return true;
        }

        protected abstract TKey ToKey(in TValue value);

        public void Clear()
        {
            _allObjects.Clear();
            _objects.Clear();
        }

        private readonly HashSet<TValue> _allObjects = new HashSet<TValue>();
        private readonly IDictionary<TKey, TValue> _objects = new Dictionary<TKey, TValue>();
    }
}