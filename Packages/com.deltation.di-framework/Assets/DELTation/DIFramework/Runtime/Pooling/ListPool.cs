using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Pooling
{
    internal static class ListPool<T>
    {
        public static List<T> Rent()
        {
            if (FreeLists.Count == 0)
                return new List<T>();

            var lastListIndex = FreeLists.Count - 1;
            var lastList = FreeLists[lastListIndex];
            FreeLists.RemoveAt(lastListIndex);
            FreeListsSet.Remove(lastList);
            lastList.Clear();
            return lastList;
        }

        public static void Return([NotNull] List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            list.Clear();
            if (FreeListsSet.Contains(list)) return;

            FreeLists.Add(list);
            FreeListsSet.Add(list);
        }

        private static readonly HashSet<List<T>> FreeListsSet = new HashSet<List<T>>();
        private static readonly List<List<T>> FreeLists = new List<List<T>>();
    }
}