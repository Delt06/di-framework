using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal static class ComponentListPool
    {
        public static List<MonoBehaviour> Rent()
        {
            if (FreeLists.Count == 0)
                return new List<MonoBehaviour>();

            var lastListIndex = FreeLists.Count - 1;
            var lastList = FreeLists[lastListIndex];
            FreeLists.RemoveAt(lastListIndex);
            FreeListsSet.Remove(lastList);
            lastList.Clear();
            return lastList;
        }

        public static void Return([NotNull] List<MonoBehaviour> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            
            list.Clear();
            if (FreeListsSet.Contains(list)) return;
            
            FreeLists.Add(list);
            FreeListsSet.Add(list);
        }

        private static readonly HashSet<List<MonoBehaviour>> FreeListsSet = new HashSet<List<MonoBehaviour>>();
        private static readonly List<List<MonoBehaviour>> FreeLists = new List<List<MonoBehaviour>>();
    }
}