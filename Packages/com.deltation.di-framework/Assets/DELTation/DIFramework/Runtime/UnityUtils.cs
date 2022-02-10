using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework
{
    internal static class UnityUtils
    {
        public static bool IsNullOrUnityNull([CanBeNull] object obj) =>
            obj is Object unityObj ? unityObj == null : obj == null;
    }
}