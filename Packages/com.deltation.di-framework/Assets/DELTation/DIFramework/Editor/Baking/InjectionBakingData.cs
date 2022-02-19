using System.Collections.Generic;
using System.Linq;

namespace DELTation.DIFramework.Editor.Baking
{
    internal static class InjectionBakingData
    {
        public const string ClassTemplate = @"// Generated automatically by DI Framework

using DELTation.DIFramework.Baking;
using UnityEngine;

namespace {0}
{{
    /// <summary>
    /// Baked injection data
    /// </summary>
    internal static class {1}
    {{
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void Bake()
        {{
            BakedInjection.DataExists = true;
            BakedInjection.Clear();
            
{2}
        }}

{3}
    }}
}}
";
        private const int SpacesInIndent = 4;


        private static readonly string[] FoldersHierarchy = { "Scripts", "DI", "Baked" };

        public static readonly string DoubleIndent = new string(' ', SpacesInIndent * 2);
        public static readonly string TripleIndent = new string(' ', SpacesInIndent * 3);

        public static string ClassName => "BakedInjectionData";

        public static string Namespace => string.Join(".", GetFoldersHierarchy().Skip(1));

        public static IReadOnlyList<string> GetFoldersHierarchy() => FoldersHierarchy;
    }
}