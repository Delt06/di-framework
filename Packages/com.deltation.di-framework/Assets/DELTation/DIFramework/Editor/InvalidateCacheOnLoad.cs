using DELTation.DIFramework.Resolution;
using UnityEditor;

namespace DELTation.DIFramework.Editor
{
    internal static class InvalidateCacheOnLoad
    {
        [InitializeOnEnterPlayMode]
        public static void InvalidateCache()
        {
            Injection.InvalidateCache();
        }
    }
}