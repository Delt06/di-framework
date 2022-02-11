using System;
using System.Collections.Generic;

namespace DELTation.DIFramework
{
    internal class TagCollection<TKey>
    {
        private readonly Dictionary<TKey, HashSet<Type>> _tags = new Dictionary<TKey, HashSet<Type>>();

        internal void Transfer<TToKey>(TKey fromKey, TagCollection<TToKey> to, TToKey toKey)
        {
            if (!_tags.TryGetValue(fromKey, out var fromTags)) return;
            if (fromTags == null) return;

            foreach (var fromTag in fromTags)
            {
                to.AddTag(toKey, fromTag);
            }
        }

        public bool HasTag(TKey key, Type tag)
        {
            if (!_tags.TryGetValue(key, out var tags)) return false;
            return tags != null && tags.Contains(tag);
        }

        public void AddTag(TKey key, Type tag)
        {
            if (!_tags.TryGetValue(key, out var tags))
                _tags[key] = tags = new HashSet<Type>();
            tags.Add(tag);
        }
    }
}